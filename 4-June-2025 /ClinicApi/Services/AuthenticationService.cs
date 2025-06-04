

using SecondWebApi.Interfaces;

using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(ITokenService tokenService, IRepository<string, User> userRepository, IEncryptionService encryptionService, ILogger<AuthenticationService> logger)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public async Task<UserLoginResponse> AuthenticateUser(UserLoginRequest userLoginRequest)
    {
        try
        {
            var user = await _userRepository.Get(userLoginRequest.Username);
            if (user is null)
            {
                _logger.LogCritical("User not Found");
                throw new Exception("No such user");
            }
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = userLoginRequest.Password,
                HashKey = user.HashKey
            });
            for (int i = 0; i < encryptedData.EncryptedData.Length; i++)
            {
                if (encryptedData.EncryptedData[i] != user.password[i])
                {
                    _logger.LogError("Invalid login attempt");
                    throw new Exception("Invalid password");
                }
            }
            var token = await _tokenService.GenerateToken(user);
            return new UserLoginResponse
            {
                Username = user.username,
                Token = token,
            };

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}