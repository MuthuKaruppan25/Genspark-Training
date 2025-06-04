using System;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;
using System.Text;

[TestFixture]
public class AuthenticationServiceTests
{
    private Mock<ITokenService> _tokenServiceMock = null!;
    private Mock<IRepository<string, User>> _userRepositoryMock = null!;
    private Mock<IEncryptionService> _encryptionServiceMock = null!;
    private Mock<ILogger<AuthenticationService>> _loggerMock = null!;
    private AuthenticationService _authService = null!;

    [SetUp]
    public void Setup()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _userRepositoryMock = new Mock<IRepository<string, User>>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();

        _authService = new AuthenticationService(
            _tokenServiceMock.Object,
            _userRepositoryMock.Object,
            _encryptionServiceMock.Object,
            _loggerMock.Object);
    }
    [Test]
    public async Task AuthenticateUser_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Username = "john",
            Password = "pass123"
        };

        var encrypted = new EncryptModel
        {
            EncryptedData = Encoding.UTF8.GetBytes(request.Password),
            HashKey = Encoding.UTF8.GetBytes("deonseioneoiwdnw2ee")
        };
        var user = new User
        {
            username = "john",
            password = encrypted.EncryptedData,
            HashKey = encrypted.HashKey,
            role = "Doctor"
        };



        _userRepositoryMock.Setup(r => r.Get("john")).ReturnsAsync(user);
        _encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encrypted);
        _tokenServiceMock.Setup(t => t.GenerateToken(user)).ReturnsAsync("token_123");

        // Act
        var result = await _authService.AuthenticateUser(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo("john"));
        Assert.That(result.Token, Is.EqualTo("token_123"));
    }

}