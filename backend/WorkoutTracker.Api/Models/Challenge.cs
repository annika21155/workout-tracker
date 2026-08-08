namespace WorkoutTracker.Api.Models;

public class Challenge
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;          // "2 Week Workout Challenge"
    public string? Description { get; set; }
    public string YoutubePlaylistUrl { get; set; } = string.Empty;
    public int DurationDays { get; set; }                       // e.g. 14
    public int CreatedByUserId { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User CreatedByUser { get; set; } = null!;
    public ICollection<ChallengeDay> Days { get; set; } = new List<ChallengeDay>();
    public ICollection<ChallengeParticipant> Participants { get; set; } = new List<ChallengeParticipant>();
}