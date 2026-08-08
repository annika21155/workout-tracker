namespace WorkoutTracker.Api.Models;

// The log: marking a specific ChallengeDay done for a specific participant.
// PointsEarned is copied from ChallengeDay.DurationMinutes at write time
// (audit trail — same pattern as your original Visit.PointsEarned).
public class DayCompletion
{
    public int Id { get; set; }
    public int ChallengeParticipantId { get; set; }
    public int ChallengeDayId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public int PointsEarned { get; set; }

    // Navigation
    public ChallengeParticipant ChallengeParticipant { get; set; } = null!;
    public ChallengeDay ChallengeDay { get; set; } = null!;
}