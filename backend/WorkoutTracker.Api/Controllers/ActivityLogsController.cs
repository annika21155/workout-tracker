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
[Authorize]
public class ActivityLogsController : ControllerBase
{
    private readonly IHubContext<LeaderboardHub> _hub;

public ActivityLogsController(AppDbContext db, IHubContext<LeaderboardHub> hub)
{
    _db = db;
    _hub = hub;
}
    private readonly AppDbContext _db;

    // GET /api/activitylogs — current user's own logs only
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = this.GetUserId();
        var logs = await _db.ActivityLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.LoggedAt)
            .Select(a => new ActivityLogResponseDto(a.Id, a.ActivityType, a.DurationMinutes, a.LoggedAt, a.PointsEarned))
            .ToListAsync();
        return Ok(logs);
    }

    // POST /api/activitylogs
    [HttpPost]
    public async Task<IActionResult> Create(CreateActivityLogDto dto)
    {
        var log = new ActivityLog
        {
            UserId = this.GetUserId(),
            ActivityType = dto.ActivityType,
            DurationMinutes = dto.DurationMinutes,
            LoggedAt = dto.LoggedAt ?? DateTime.UtcNow,
            PointsEarned = dto.DurationMinutes, // 1 point per minute, audit trail
        };
        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("LeaderboardUpdated");

        return Ok(new ActivityLogResponseDto(log.Id, log.ActivityType, log.DurationMinutes, log.LoggedAt, log.PointsEarned));
    }

    // DELETE /api/activitylogs/{id} — only your own
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var log = await _db.ActivityLogs.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (log is null) return NotFound();

        _db.ActivityLogs.Remove(log);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}