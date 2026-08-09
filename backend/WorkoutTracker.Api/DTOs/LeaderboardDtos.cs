namespace WorkoutTracker.Api.DTOs;

public record LeaderboardEntryDto(int UserId, string Username, int TotalPoints, int Rank);