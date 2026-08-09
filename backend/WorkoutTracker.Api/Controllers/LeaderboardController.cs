using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.DTOs;
using WorkoutTracker.Api.Helpers;
using WorkoutTracker.Api.Models;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaderboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public LeaderboardController(AppDbContext db) => _db = db;

    // GET /api/leaderboard — this month, friends + self, ranked by total points
    [HttpGet]
    public async Task<IActionResult> GetMonthlyLeaderboard()
    {
        var userId = this.GetUserId();
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var friendIds = await _db.Friendships
            .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Status == "Accepted")
            .Select(f => f.UserId == userId ? f.FriendUserId : f.UserId)
            .ToListAsync();

        var relevantUserIds = friendIds.Append(userId).Distinct().ToList();

        var challengePoints = await _db.DayCompletions
            .Include(dc => dc.ChallengeParticipant)
            .Where(dc => relevantUserIds.Contains(dc.ChallengeParticipant.UserId) && dc.CompletedAt >= monthStart)
            .GroupBy(dc => dc.ChallengeParticipant.UserId)
            .Select(g => new { UserId = g.Key, Points = g.Sum(dc => dc.PointsEarned) })
            .ToListAsync();

        var activityPoints = await _db.ActivityLogs
            .Where(a => relevantUserIds.Contains(a.UserId) && a.LoggedAt >= monthStart)
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, Points = g.Sum(a => a.PointsEarned) })
            .ToListAsync();

        var totals = relevantUserIds.ToDictionary(id => id, id => 0);
        foreach (var cp in challengePoints) totals[cp.UserId] = totals.GetValueOrDefault(cp.UserId) + cp.Points;
        foreach (var ap in activityPoints) totals[ap.UserId] = totals.GetValueOrDefault(ap.UserId) + ap.Points;

        var users = await _db.Users.Where(u => relevantUserIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Username);

        var ranked = totals
            .OrderByDescending(t => t.Value)
            .Select((t, index) => new LeaderboardEntryDto(t.Key, users[t.Key], t.Value, index + 1))
            .ToList();

        return Ok(ranked);
    }
}