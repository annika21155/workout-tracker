namespace WorkoutTracker.Api.DTOs;

public record SendFriendRequestDto(string FriendEmail);

public record FriendshipResponseDto(int Id, int FriendUserId, string FriendUsername, string Status);