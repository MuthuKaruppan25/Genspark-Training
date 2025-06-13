using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class AuthenticationServiceTests
{
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();
    private readonly Mock<IRepository<Guid, Seeker>> _seekerRepoMock = new();
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _service = new AuthenticationService(
            _userRepoMock.Object,
            _tokenServiceMock.Object,
            _seekerRepoMock.Object,
            _encryptionServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_Success()
    {
        var user = new User
        {
            guid = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = new byte[] { 1, 2, 3 },
            HashKey = new byte[] { 4, 5, 6 },
            Role = "Seeker"
        };
        var seeker = new Seeker
        {
            guid = Guid.NewGuid(),
            UserId = user.guid
        };
        var dto = new LoginRequestDto { Username = "testuser", Password = "pw", ConnectionId = "conn-123" };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _encryptionServiceMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = user.PasswordHash });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).ReturnsAsync("access-token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken(user)).ReturnsAsync("refresh-token");
        _seekerRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Seeker> { seeker });
        _seekerRepoMock.Setup(x => x.Update(seeker.guid, It.IsAny<Seeker>())).ReturnsAsync(seeker);

        var result = await _service.LoginAsync(dto);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
       

    }


    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsAuthenticationException()
    {
        var user = new User
        {
            guid = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = new byte[] { 1, 2, 3 },
            HashKey = new byte[] { 4, 5, 6 }
        };
        var dto = new LoginRequestDto { Username = "testuser", Password = "wrongpw" };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _encryptionServiceMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = new byte[] { 9, 9, 9 } });

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthenticationException()
    {
        var dto = new LoginRequestDto { Username = "nouser", Password = "pw" };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.LoginAsync(dto));
    }

    [Fact]
    public async Task RefreshTokenAsync_Success()
    {
        var user = new User
        {
            guid = Guid.NewGuid(),
            Username = "testuser",
            RefreshToken = "refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _tokenServiceMock.Setup(x => x.GetPrincipalFromExpiredToken("expired")).Returns(principal);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).ReturnsAsync("new-access-token");

        var result = await _service.RefreshTokenAsync("expired", "refresh-token");

        Assert.Equal("new-access-token", result);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidToken_ThrowsAuthenticationException()
    {
        _tokenServiceMock.Setup(x => x.GetPrincipalFromExpiredToken("expired")).Returns((ClaimsPrincipal)null);

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.RefreshTokenAsync("expired", "refresh-token"));
    }

    [Fact]
    public async Task RefreshTokenAsync_UserNotFound_ThrowsAuthenticationException()
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _tokenServiceMock.Setup(x => x.GetPrincipalFromExpiredToken("expired")).Returns(principal);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.RefreshTokenAsync("expired", "refresh-token"));
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredRefreshToken_ThrowsAuthenticationException()
    {
        var user = new User
        {
            guid = Guid.NewGuid(),
            Username = "testuser",
            RefreshToken = "refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(-1)
        };
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "testuser") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        _tokenServiceMock.Setup(x => x.GetPrincipalFromExpiredToken("expired")).Returns(principal);
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.RefreshTokenAsync("expired", "refresh-token"));
    }

    [Fact]
    public async Task LogoutAsync_Success()
    {
        var user = new User
        {
            guid = Guid.NewGuid(),
            Username = "testuser",
            RefreshToken = "refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { user });
        _userRepoMock.Setup(x => x.Update(user.guid, It.IsAny<User>())).ReturnsAsync(user);

        await _service.LogoutAsync("testuser");

        _userRepoMock.Verify(x => x.Update(user.guid, It.Is<User>(u =>
            u.RefreshToken == string.Empty &&
            u.RefreshTokenExpiryTime == DateTime.MinValue
        )), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_UserNotFound_ThrowsRecordNotFoundException()
    {
        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.LogoutAsync("nouser"));
    }

    [Fact]
    public async Task VerifyPassword_Success()
    {
        var password = "pw";
        var hashKey = new byte[] { 1, 2, 3 };
        var hash = new byte[] { 4, 5, 6 };
        _encryptionServiceMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = hash });

        var result = await _service.VerifyPassword(password, hash, hashKey);

        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPassword_Failure()
    {
        var password = "pw";
        var hashKey = new byte[] { 1, 2, 3 };
        var hash = new byte[] { 4, 5, 6 };
        _encryptionServiceMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { EncryptedData = new byte[] { 9, 9, 9 } });

        var result = await _service.VerifyPassword(password, hash, hashKey);

        Assert.False(result);
    }

    [Fact]
    public async Task VerifyPassword_ThrowsEncryptionException()
    {
        var password = "pw";
        var hashKey = new byte[] { 1, 2, 3 };
        var hash = new byte[] { 4, 5, 6 };
        _encryptionServiceMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ThrowsAsync(new Exception("encryption error"));

        await Assert.ThrowsAsync<EncryptionException>(() => _service.VerifyPassword(password, hash, hashKey));
    }
}