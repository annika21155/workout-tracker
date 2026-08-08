using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Api.DTOs;
using WorkoutTracker.Api.Services;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result is null) return Conflict(new { message = "Email already in use." });
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null) return Unauthorized(new { message = "Invalid email or password." });
        return Ok(result);
    }
}