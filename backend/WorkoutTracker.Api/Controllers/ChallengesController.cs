using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.DTOs;
using WorkoutTracker.Api.Helpers;
using WorkoutTracker.Api.Models;
using Microsoft.AspNetCore.SignalR;
using WorkoutTracker.Api.Hubs;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChallengesController : ControllerBase
{
    private readonly IHubContext<LeaderboardHub> _hub;

public ChallengesController(AppDbContext db, IHubContext<LeaderboardHub> hub)
{
    _db = db;
    _hub = hub;
}
    private readonly AppDbContext _db;

    // GET /api/challenges — public, no auth required
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var challenges = await _db.Challenges
            .Where(c => c.IsPublic)
            .Select(c => new ChallengeSummaryDto(c.Id, c.Title, c.Description, c.DurationDays, c.IsPublic, c.Participants.Count))
            .ToListAsync();
        return Ok(challenges);
    }

    // GET /api/challenges/{id} — public
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var challenge = await _db.Challenges.Include(c => c.Days).FirstOrDefaultAsync(c => c.Id == id);
        if (challenge is null) return NotFound();

        var dto = new ChallengeDetailDto(
            challenge.Id, challenge.Title, challenge.Description, challenge.YoutubePlaylistUrl,
            challenge.DurationDays, challenge.IsPublic,
            challenge.Days.OrderBy(d => d.DayNumber)
                .Select(d => new ChallengeDayResponseDto(d.Id, d.DayNumber, d.VideoUrl, d.VideoTitle, d.DurationMinutes))
                .ToList()
        );
        return Ok(dto);
    }

    // POST /api/challenges — requires auth
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateChallengeDto dto)
    {
        var challenge = new Challenge
        {
            Title = dto.Title,
            Description = dto.Description,
            YoutubePlaylistUrl = dto.YoutubePlaylistUrl,
            DurationDays = dto.DurationDays,
            IsPublic = dto.IsPublic,
            CreatedByUserId = this.GetUserId(),
            Days = dto.Days.Select(d => new ChallengeDay
            {
                DayNumber = d.DayNumber,
                VideoUrl = d.VideoUrl,
                VideoTitle = d.VideoTitle,
                DurationMinutes = d.DurationMinutes
            }).ToList()
        };

        _db.Challenges.Add(challenge);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = challenge.Id }, new { id = challenge.Id });
    }

    // POST /api/challenges/{id}/join — join solo or as a friend pairing
    [HttpPost("{id}/join")]
    [Authorize]
    public async Task<IActionResult> Join(int id)
    {
        var exists = await _db.Challenges.AnyAsync(c => c.Id == id);
        if (!exists) return NotFound();

        var userId = this.GetUserId();
        var alreadyJoined = await _db.ChallengeParticipants.AnyAsync(cp => cp.ChallengeId == id && cp.UserId == userId);
        if (alreadyJoined) return Conflict(new { message = "Already joined this challenge." });

        var participant = new ChallengeParticipant { ChallengeId = id, UserId = userId };
        _db.ChallengeParticipants.Add(participant);
        await _db.SaveChangesAsync();

        return Ok(new { participantId = participant.Id });
    }

    // GET /api/challenges/{id}/today — the "Today's Video" card
    [HttpGet("{id}/today")]
    [Authorize]
    public async Task<IActionResult> GetTodaysVideo(int id)
    {
        var userId = this.GetUserId();
        var participant = await _db.ChallengeParticipants
            .Include(cp => cp.Completions)
            .FirstOrDefaultAsync(cp => cp.ChallengeId == id && cp.UserId == userId);
        if (participant is null) return BadRequest(new { message = "You haven't joined this challenge." });

        var completedDayIds = participant.Completions.Select(c => c.ChallengeDayId).ToHashSet();
        var nextDay = await _db.ChallengeDays
            .Where(d => d.ChallengeId == id && !completedDayIds.Contains(d.Id))
            .OrderBy(d => d.DayNumber)
            .FirstOrDefaultAsync();

        if (nextDay is null) return Ok(new { message = "Challenge complete! No more days left." });

        return Ok(new ChallengeDayResponseDto(nextDay.Id, nextDay.DayNumber, nextDay.VideoUrl, nextDay.VideoTitle, nextDay.DurationMinutes));
    }

    // POST /api/challenges/{challengeId}/days/{dayId}/complete — mark a day done
    [HttpPost("{challengeId}/days/{dayId}/complete")]
    [Authorize]
    public async Task<IActionResult> CompleteDay(int challengeId, int dayId)
    {
        var userId = this.GetUserId();
        var participant = await _db.ChallengeParticipants
            .FirstOrDefaultAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);
        if (participant is null) return BadRequest(new { message = "You haven't joined this challenge." });

        var day = await _db.ChallengeDays.FirstOrDefaultAsync(d => d.Id == dayId && d.ChallengeId == challengeId);
        if (day is null) return NotFound();

        var alreadyDone = await _db.DayCompletions
            .AnyAsync(dc => dc.ChallengeParticipantId == participant.Id && dc.ChallengeDayId == dayId);
        if (alreadyDone) return Conflict(new { message = "Day already completed." });

        var completion = new DayCompletion
        {
            ChallengeParticipantId = participant.Id,
            ChallengeDayId = dayId,
            PointsEarned = day.DurationMinutes, // audit trail — copied at write time
        };
        _db.DayCompletions.Add(completion);
        participant.CurrentStreak += 1; // simplified — no date-gap checking for MVP

        await _db.SaveChangesAsync();

        // notify friends in this challenge that the leaderboard changed.
        await _hub.Clients.All.SendAsync("LeaderboardUpdated");

        return Ok(new { pointsEarned = completion.PointsEarned, currentStreak = participant.CurrentStreak });
    }
}