using LoginApi.Auth.Models;
using LoginApi.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginApi.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await auth.LoginAsync(request);
        if (result.StatusCode == 200 && result.Response is not null && result.RefreshToken is not null)
        {
            AppendRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt ?? DateTimeOffset.UtcNow.AddHours(8));
            return Ok(result.Response);
        }

        return result.StatusCode == 423
            ? StatusCode(423, new { error = result.Error })
            : Unauthorized(new { error = result.Error });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new { error = "Refresh token missing." });
        }

        var result = await auth.RefreshAsync(refreshToken);
        if (result.StatusCode == 200 && result.Response is not null)
        {
            if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                AppendRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt ?? DateTimeOffset.UtcNow.AddHours(8));
            }

            return Ok(result.Response);
        }

        return Unauthorized(new { error = result.Error });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await auth.LogoutAsync(refreshToken);
        }

        Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = false
        });

        return NoContent();
    }

    private void AppendRefreshTokenCookie(string token, DateTimeOffset expiresAt) =>
        Response.Cookies.Append(RefreshTokenCookie, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = false,
            IsEssential = true,
            Expires = expiresAt
        });
}
