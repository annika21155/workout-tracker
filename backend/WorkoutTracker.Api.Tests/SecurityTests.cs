using Microsoft.Extensions.Configuration;
using WorkoutTracker.Api.Services;
using WorkoutTracker.Api.Models;
using Xunit;

namespace WorkoutTracker.Api.Tests;

public class SecurityTests
{
    [Fact]
    public void PasswordHashing_HashedPassword_VerifiesCorrectly()
    {
        var plainPassword = "TestPassword123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        var result = BCrypt.Net.BCrypt.Verify(plainPassword, hash);

        Assert.True(result);
    }

    [Fact]
    public void PasswordHashing_WrongPassword_FailsVerification()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!");

        var result = BCrypt.Net.BCrypt.Verify("WrongPassword", hash);

        Assert.False(result);
    }

    [Fact]
    public void TokenService_GenerateToken_ProducesNonEmptyToken()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "test-key-that-is-at-least-32-characters-long-for-hs256" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:ExpiryMinutes", "60" },
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenService = new TokenService(config);
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com" };

        var token = tokenService.GenerateToken(user);

        Assert.False(string.IsNullOrEmpty(token));
        Assert.Contains(".", token); // JWTs are three dot-separated segments
    }
}