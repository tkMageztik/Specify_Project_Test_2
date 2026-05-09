using LoginApi.Auth.Models;
using LoginApi.Users.Models;
using LoginApi.Users.Repositories;

namespace LoginApi.Auth.Services;

public class AuthService(
    IUserRepository users,
    IJwtTokenService jwt,
    IConfiguration config) : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized();
        }

        if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
        {
            return Locked();
        }

        if (user.LockoutUntil.HasValue && user.LockoutUntil <= DateTime.UtcNow)
        {
            user.LockoutUntil = null;
            user.FailedAttempts = 0;
            user.Status = UserStatus.Active;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= MaxFailedAttempts)
            {
                user.Status = UserStatus.Locked;
                user.LockoutUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            }

            await users.UpdateAsync(user);
            return user.Status == UserStatus.Locked ? Locked() : Unauthorized();
        }

        user.FailedAttempts = 0;
        user.LockoutUntil = null;
        user.Status = UserStatus.Active;
        await users.UpdateAsync(user);

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var token = await users.FindRefreshTokenAsync(refreshToken);
        if (token is null || !token.IsValid)
        {
            return new AuthResult(null, "Invalid or expired refresh token.", 401);
        }

        await users.RevokeRefreshTokenAsync(token);
        return await IssueTokensAsync(token.User);
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await users.FindRefreshTokenAsync(refreshToken);
        if (token is null)
        {
            return false;
        }

        await users.RevokeRefreshTokenAsync(token);
        return true;
    }

    private async Task<AuthResult> IssueTokensAsync(User user)
    {
        var accessToken = jwt.GenerateAccessToken(user);
        var refreshTokenValue = jwt.GenerateRefreshToken();
        var refreshExpiryHours = int.Parse(config["Jwt:RefreshTokenExpiryHours"] ?? "8");
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(refreshExpiryHours);

        await users.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = refreshTokenExpiresAt.UtcDateTime
        });

        return new AuthResult(
            new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresIn = jwt.AccessTokenExpirySeconds
            },
            null,
            200,
            refreshTokenValue,
            refreshTokenExpiresAt);
    }

    private static AuthResult Unauthorized() =>
        new(null, "Invalid email or password.", 401);

    private static AuthResult Locked() =>
        new(null, "Account temporarily locked. Try again in 15 minutes.", 423);
}
