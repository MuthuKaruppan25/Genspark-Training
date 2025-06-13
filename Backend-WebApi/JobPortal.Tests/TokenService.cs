using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

public class TokenServiceTests
{
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly TokenService _service;
    private readonly string _jwtKey = "super_secret_jwt_key_1234567890!";

    public TokenServiceTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Keys:JwtTokenKey"]).Returns(_jwtKey);
        _service = new TokenService(configMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public async Task GenerateAccessToken_Returns_Valid_JWT()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "testuser", Role = "Admin" };

        var token = await _service.GenerateAccessToken(user);

        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        Assert.Equal("testuser", jwt.Claims.First(c => c.Type == "nameid").Value);
        Assert.Equal("Admin", jwt.Claims.First(c => c.Type == "role").Value);
    }


    [Fact]
    public async Task GenerateRefreshToken_Returns_And_UpdatesUser()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "testuser" };
        _userRepoMock.Setup(x => x.Update(user.guid, user)).ReturnsAsync(user);

        var refreshToken = await _service.GenerateRefreshToken(user);

        Assert.False(string.IsNullOrEmpty(refreshToken));
        Assert.Equal(user.RefreshToken, refreshToken);
        Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow);
        _userRepoMock.Verify(x => x.Update(user.guid, user), Times.Once);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ValidToken_ReturnsPrincipal()
    {
        var user = new User { guid = Guid.NewGuid(), Username = "testuser", Role = "User" };

        var token = _service.GenerateAccessToken(user).Result;

        var principal = _service.GetPrincipalFromExpiredToken(token);

        Assert.NotNull(principal);
        Assert.Equal("testuser", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("User", principal.FindFirst(ClaimTypes.Role)?.Value);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_InvalidToken_Throws()
    {
        var invalidToken = "invalid.token.value";
        Assert.Throws<ArgumentException>(() => _service.GetPrincipalFromExpiredToken(invalidToken));
    }
}