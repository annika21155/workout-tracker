namespace WorkoutTracker.Api.DTOs;

public record RegisterDto(string Username, string Email, string Password);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, int UserId, string Username, string Email);