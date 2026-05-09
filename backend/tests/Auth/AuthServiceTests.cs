using LoginApi.Auth.Models;
using LoginApi.Auth.Services;
using LoginApi.Users.Models;
using LoginApi.Users.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace LoginApi.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IJwtTokenService> _jwt = new();
    private readonly Mock<IConfiguration> _config = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _config.Setup(config => config["Jwt:RefreshTokenExpiryHours"]).Returns("8");
        _jwt.Setup(jwt => jwt.GenerateAccessToken(It.IsAny<User>())).Returns("fake-access-token");
        _jwt.Setup(jwt => jwt.GenerateRefreshToken()).Returns("fake-refresh-token");
        _jwt.Setup(jwt => jwt.AccessTokenExpirySeconds).Returns(900);
        _sut = new AuthService(_users.Object, _jwt.Object, _config.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAccessToken()
    {
        var user = CreateActiveUser("test@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));
        _users.Setup(repository => repository.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _users.Setup(repository => repository.UpdateAsync(user)).Returns(Task.CompletedTask);
        _users.Setup(repository => repository.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "password123" });

        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Response);
        Assert.Equal("fake-access-token", result.Response!.AccessToken);
        Assert.Equal(900, result.Response.ExpiresIn);
        Assert.Equal("fake-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_SuccessfulLogin_ResetsFailedAttempts()
    {
        var user = CreateActiveUser("test@example.com", BCrypt.Net.BCrypt.HashPassword("password123"));
        user.FailedAttempts = 3;
        _users.Setup(repository => repository.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _users.Setup(repository => repository.UpdateAsync(user)).Returns(Task.CompletedTask);
        _users.Setup(repository => repository.AddRefreshTokenAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        await _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "password123" });

        Assert.Equal(0, user.FailedAttempts);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsGenericError()
    {
        var user = CreateActiveUser("test@example.com", BCrypt.Net.BCrypt.HashPassword("correctpassword"));
        _users.Setup(repository => repository.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _users.Setup(repository => repository.UpdateAsync(user)).Returns(Task.CompletedTask);

        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "wrongpassword" });

        Assert.Equal(401, result.StatusCode);
        Assert.Equal("Invalid email or password.", result.Error);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task LoginAsync_FifthFailedAttempt_LocksAccount()
    {
        var user = CreateActiveUser("test@example.com", BCrypt.Net.BCrypt.HashPassword("correct"));
        user.FailedAttempts = 4;
        _users.Setup(repository => repository.FindByEmailAsync("test@example.com")).ReturnsAsync(user);
        _users.Setup(repository => repository.UpdateAsync(user)).Returns(Task.CompletedTask);

        var before = DateTime.UtcNow.AddMinutes(14);
        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@example.com", Password = "wrong" });

        Assert.Equal(423, result.StatusCode);
        Assert.Equal(UserStatus.Locked, user.Status);
        Assert.NotNull(user.LockoutUntil);
        Assert.True(user.LockoutUntil > before);
    }

    private static User CreateActiveUser(string email, string hash) => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        PasswordHash = hash,
        Status = UserStatus.Active,
        FailedAttempts = 0
    };
}
