public interface IGoogleOAuthService
{
    Task<string?> GetGoogleLoginUrl(string role);
    Task<string?> ExchangeCodeForTokenAsync(string code,string state);
}
