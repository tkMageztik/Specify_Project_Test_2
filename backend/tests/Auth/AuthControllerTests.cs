using LoginApi.Auth.Controllers;
using LoginApi.Auth.Models;
using LoginApi.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LoginApi.Tests.Auth;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _auth = new();

    [Fact]
    public async Task Login_WhenSuccessful_ReturnsOkAndSetsCookie()
    {
        var controller = CreateController();
        _auth.Setup(service => service.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new AuthResult(
                new LoginResponse { AccessToken = "access-token", ExpiresIn = 900 },
                null,
                200,
                "refresh-token",
                DateTimeOffset.UtcNow.AddHours(8)));

        var result = await controller.Login(new LoginRequest { Email = "user@example.com", Password = "Password123!" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.Equal("access-token", response.AccessToken);
        Assert.Contains("refresh_token=refresh-token", controller.Response.Headers.SetCookie.ToString());
    }

    [Fact]
    public async Task Login_WhenLocked_ReturnsLockedStatusCode()
    {
        var controller = CreateController();
        _auth.Setup(service => service.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new AuthResult(null, "Account temporarily locked. Try again in 15 minutes.", 423));

        var result = await controller.Login(new LoginRequest { Email = "user@example.com", Password = "Password123!" });

        var locked = Assert.IsType<ObjectResult>(result);
        Assert.Equal(423, locked.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_ReturnsUnauthorized()
    {
        var controller = CreateController();

        var result = await controller.Refresh();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public async Task Logout_ClearsCookieAndReturnsNoContent()
    {
        var controller = CreateController("refresh_token=refresh-token");
        _auth.Setup(service => service.LogoutAsync("refresh-token")).ReturnsAsync(true);

        var result = await controller.Logout();

        Assert.IsType<NoContentResult>(result);
        Assert.Contains("refresh_token=", controller.Response.Headers.SetCookie.ToString());
    }

    private AuthController CreateController(string? cookieHeader = null)
    {
        var controller = new AuthController(_auth.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        if (!string.IsNullOrWhiteSpace(cookieHeader))
        {
            controller.ControllerContext.HttpContext.Request.Headers.Cookie = cookieHeader;
        }

        return controller;
    }
}
