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
public class FriendshipsController : ControllerBase
{
    private readonly AppDbContext _db;
    public FriendshipsController(AppDbContext db) => _db = db;

    // GET /api/friendships — accepted friends only
    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        var userId = this.GetUserId();

        var friendships = await _db.Friendships
            .Include(f => f.User)
            .Include(f => f.FriendUser)
            .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Status == "Accepted")
            .ToListAsync();

        var result = friendships.Select(f =>
        {
            var friend = f.UserId == userId ? f.FriendUser : f.User;
            return new FriendshipResponseDto(f.Id, friend.Id, friend.Username, f.Status);
        });

        return Ok(result);
    }

    // GET /api/friendships/pending — requests sent TO me, awaiting my response
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var userId = this.GetUserId();
        var pending = await _db.Friendships
            .Include(f => f.User)
            .Where(f => f.FriendUserId == userId && f.Status == "Pending")
            .Select(f => new FriendshipResponseDto(f.Id, f.User.Id, f.User.Username, f.Status))
            .ToListAsync();
        return Ok(pending);
    }

    // POST /api/friendships — send a request by email
    [HttpPost]
    public async Task<IActionResult> SendRequest(SendFriendRequestDto dto)
    {
        var userId = this.GetUserId();
        var friend = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.FriendEmail);
        if (friend is null) return NotFound(new { message = "No user found with that email." });
        if (friend.Id == userId) return BadRequest(new { message = "You can't friend yourself." });

        var exists = await _db.Friendships.AnyAsync(f =>
            (f.UserId == userId && f.FriendUserId == friend.Id) ||
            (f.UserId == friend.Id && f.FriendUserId == userId));
        if (exists) return Conflict(new { message = "Friendship already exists or is pending." });

        var friendship = new Friendship { UserId = userId, FriendUserId = friend.Id, Status = "Pending" };
        _db.Friendships.Add(friendship);
        await _db.SaveChangesAsync();

        return Ok(new { friendshipId = friendship.Id });
    }

    // POST /api/friendships/{id}/accept
    [HttpPost("{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var userId = this.GetUserId();
        var friendship = await _db.Friendships.FirstOrDefaultAsync(f => f.Id == id && f.FriendUserId == userId);
        if (friendship is null) return NotFound();

        friendship.Status = "Accepted";
        await _db.SaveChangesAsync();
        return Ok();
    }
}