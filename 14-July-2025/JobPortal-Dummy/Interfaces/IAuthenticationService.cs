
public interface IAuthenticationService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
    Task<string> RefreshTokenAsync(string expiredToken,string refreshToken);
    Task LogoutAsync(string username);
    Task<bool> VerifyPassword(string password, byte[] storedHash, byte[] storedKey);

}