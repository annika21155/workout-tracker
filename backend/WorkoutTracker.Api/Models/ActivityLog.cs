namespace WorkoutTracker.Api.Models;

// Manual entries outside any challenge — "20 min walk"
public class ActivityLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActivityType { get; set; } = string.Empty;  // enum-as-string: "Walk", "Run", "Gym", "Yoga", "Other"
    public int DurationMinutes { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public int PointsEarned { get; set; }                     // = DurationMinutes, stored at write time

    // Navigation
    public User User { get; set; } = null!;
}