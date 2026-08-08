namespace WorkoutTracker.Api.Models;

// A user's membership in a challenge. Two users sharing a ChallengeId
// is how "pairing with a friend" works — no separate pairing table needed.
public class ChallengeParticipant
{
    public int Id { get; set; }
    public int ChallengeId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public int CurrentStreak { get; set; } = 0;

    // Navigation
    public Challenge Challenge { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<DayCompletion> Completions { get; set; } = new List<DayCompletion>();
}