namespace WorkoutTracker.Api.DTOs;

public record ChallengeDayInputDto(int DayNumber, string VideoUrl, string VideoTitle, int DurationMinutes);

public record CreateChallengeDto(
    string Title,
    string? Description,
    string YoutubePlaylistUrl,
    int DurationDays,
    bool IsPublic,
    List<ChallengeDayInputDto> Days
);

public record ChallengeSummaryDto(int Id, string Title, string? Description, int DurationDays, bool IsPublic, int ParticipantCount);

public record ChallengeDayResponseDto(int Id, int DayNumber, string VideoUrl, string VideoTitle, int DurationMinutes);

public record ChallengeDetailDto(
    int Id,
    string Title,
    string? Description,
    string YoutubePlaylistUrl,
    int DurationDays,
    bool IsPublic,
    List<ChallengeDayResponseDto> Days
);