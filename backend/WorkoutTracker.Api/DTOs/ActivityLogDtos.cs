namespace WorkoutTracker.Api.DTOs;

public record CreateActivityLogDto(string ActivityType, int DurationMinutes, DateTime? LoggedAt);

public record ActivityLogResponseDto(int Id, string ActivityType, int DurationMinutes, DateTime LoggedAt, int PointsEarned);