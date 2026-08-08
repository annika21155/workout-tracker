namespace WorkoutTracker.Api.Models;

// Needed so the leaderboard can be scoped to "my friends" rather than everyone.
public class Friendship
{
    public int Id { get; set; }
    public int UserId { get; set; }         // requester
    public int FriendUserId { get; set; }   // recipient
    public string Status { get; set; } = "Pending"; // "Pending" | "Accepted"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public User FriendUser { get; set; } = null!;
}