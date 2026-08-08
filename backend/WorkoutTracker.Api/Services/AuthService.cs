using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Api.DTOs;
using WorkoutTracker.Api.Models;

namespace WorkoutTracker.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return null; // caller returns 409 Conflict

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);
        return new AuthResponseDto(token, user.Id, user.Username, user.Email);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null) return null; // caller returns 401

        var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!valid) return null; // caller returns 401

        var token = _tokenService.GenerateToken(user);
        return new AuthResponseDto(token, user.Id, user.Username, user.Email);
    }
}