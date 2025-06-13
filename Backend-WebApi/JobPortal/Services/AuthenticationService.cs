using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using log4net;
using System.Security.Claims;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<Guid, User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRepository<Guid, Seeker> _seekerRepository;
    private readonly IEncryptionService _encryptionService;
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public AuthenticationService(
        IRepository<Guid, User> userRepository,
        ITokenService tokenService,
        IRepository<Guid, Seeker> seekerRepository,
        IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _seekerRepository = seekerRepository;
        _encryptionService = encryptionService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
    {
        try
        {
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username == loginRequestDto.Username);
            if (user == null || !await VerifyPassword(loginRequestDto.Password, user.PasswordHash, user.HashKey))
                throw new AuthenticationException("Invalid username or password");

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            _dataLogger.Info($"User logged in: {user.Username}");
            if (user.Role == "Seeker" && !string.IsNullOrEmpty(loginRequestDto.ConnectionId))
            {
                var seekers = await _seekerRepository.GetAll();
                var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
                if (seeker == null)
                    throw new RecordNotFoundException("Seeker is not found");
                seeker.ConnectionId = loginRequestDto.ConnectionId;
                await _seekerRepository.Update(seeker.guid, seeker);
            }
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Login failed", ex);
            throw new AuthenticationException("Login failed", ex);
        }
    }

    public async Task<string> RefreshTokenAsync(string expiredToken, string refreshToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(expiredToken);
            if (principal == null)
                throw new AuthenticationException("Invalid expired access token");

            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new AuthenticationException("Invalid token");

            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new AuthenticationException("Invalid or expired refresh token");

            var newAccessToken = await _tokenService.GenerateAccessToken(user);

            _dataLogger.Info($"Token refreshed for user: {username}");

            return newAccessToken;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Token refresh failed", ex);
            throw new AuthenticationException("Token refresh failed", ex);
        }
    }

    public async Task LogoutAsync(string username)
    {
        try
        {
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                throw new RecordNotFoundException("User not found");

            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await _userRepository.Update(user.guid, user);

            _dataLogger.Info($"User logged out: {username}");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Logout failed", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Logout failed", ex);
            throw new AuthenticationException("Logout failed", ex);
        }
    }

    public async Task<bool> VerifyPassword(string password, byte[] storedHash, byte[] storedKey)
    {
        try
        {
            var encrypted = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = password,
                HashKey = storedKey
            });

            return encrypted.EncryptedData!.SequenceEqual(storedHash);
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Password verification failed", ex);
            throw new EncryptionException("Password verification failed", ex);
        }
    }
}
