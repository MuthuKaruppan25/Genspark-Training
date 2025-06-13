using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;

public class AuthenticationControllerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _controller = new AuthenticationController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenSuccess()
    {
        var loginDto = new LoginRequestDto();
        var response = new LoginResponseDto();

        _authServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(response);

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.Login(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Login data is required.", badRequest.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenAuthenticationException()
    {
        var loginDto = new LoginRequestDto();
        _authServiceMock.Setup(s => s.LoginAsync(loginDto))
            .ThrowsAsync(new AuthenticationException("Invalid credentials"));

        var result = await _controller.Login(loginDto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var errorObj = unauthorized.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Invalid credentials", errorValue);
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenSuccess()
    {
        var refreshDto = new RefreshTokenRequestDto { ExpiredToken = "expired", RefreshToken = "refresh" };
        var newToken = "new-access-token";

        _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto.ExpiredToken, refreshDto.RefreshToken))
            .ReturnsAsync(newToken);

        var result = await _controller.RefreshToken(refreshDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(newToken, okResult.Value.GetType().GetProperty("AccessToken")?.GetValue(okResult.Value));
    }

    [Fact]
    public async Task RefreshToken_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.RefreshToken(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Refresh token data is required.", badRequest.Value);
    }

    [Fact]
    public async Task RefreshToken_ReturnsUnauthorized_WhenAuthenticationException()
    {
        var refreshDto = new RefreshTokenRequestDto { ExpiredToken = "expired", RefreshToken = "refresh" };
        _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto.ExpiredToken, refreshDto.RefreshToken))
            .ThrowsAsync(new AuthenticationException("Invalid refresh"));

        var result = await _controller.RefreshToken(refreshDto);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var errorObj = unauthorized.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Invalid refresh", errorValue);
    }

    [Fact]
    public async Task Logout_ReturnsOk_WhenSuccess()
    {
        var logoutDto = new LogoutRequestDto { username = "user" };

        _authServiceMock.Setup(s => s.LogoutAsync(logoutDto.username)).Returns(Task.CompletedTask);

        var result = await _controller.Logout(logoutDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Logout successful.", msgValue);
    }

    [Fact]
    public async Task Logout_ReturnsBadRequest_WhenNullOrUsernameMissing()
    {
        var result = await _controller.Logout(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username is required for logout.", badRequest.Value);

        var result2 = await _controller.Logout(new LogoutRequestDto { username = "" });

        var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
        Assert.Equal("Username is required for logout.", badRequest2.Value);
    }

    [Fact]
    public async Task Logout_ReturnsNotFound_WhenRecordNotFound()
    {
        var logoutDto = new LogoutRequestDto { username = "user" };
        _authServiceMock.Setup(s => s.LogoutAsync(logoutDto.username))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.Logout(logoutDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task Logout_Returns500_WhenApplicationException()
    {
        var logoutDto = new LogoutRequestDto { username = "user" };
        _authServiceMock.Setup(s => s.LogoutAsync(logoutDto.username))
            .ThrowsAsync(new ApplicationException("App error"));

        var result = await _controller.Logout(logoutDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("App error", errorValue);
    }
}