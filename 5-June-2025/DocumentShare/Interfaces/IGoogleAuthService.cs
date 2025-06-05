using DocumentShare.Models;

public interface IGoogleOAuthService
{
    Task<string?> GetGoogleLoginUrl(UserRegisterDto userRegisterDto);
    Task<string?> ExchangeCodeForTokenAsync(string code,string state);
}
