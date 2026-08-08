namespace WorkoutTracker.Api.Models;

// One row per video in the playlist — the actual daily entries
public class ChallengeDay
{
    public int Id { get; set; }
    public int ChallengeId { get; set; }
    public int DayNumber { get; set; }          // 1, 2, 3...
    public string VideoUrl { get; set; } = string.Empty;
    public string VideoTitle { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }    // drives points

    // Navigation
    public Challenge Challenge { get; set; } = null!;
    public ICollection<DayCompletion> Completions { get; set; } = new List<DayCompletion>();
}