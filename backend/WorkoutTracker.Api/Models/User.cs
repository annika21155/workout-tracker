namespace WorkoutTracker.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ChallengeParticipant> ChallengeParticipations { get; set; } = new List<ChallengeParticipant>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public ICollection<Challenge> CreatedChallenges { get; set; } = new List<Challenge>();

    // Friendships where this user initiated the request
    public ICollection<Friendship> FriendshipsInitiated { get; set; } = new List<Friendship>();
    // Friendships where this user was the target
    public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();
}