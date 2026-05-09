using LoginApi.Auth.Models;

namespace LoginApi.Auth.Services;

public sealed record AuthResult(
    LoginResponse? Response,
    string? Error,
    int StatusCode,
    string? RefreshToken = null,
    DateTimeOffset? RefreshTokenExpiresAt = null);

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
}
