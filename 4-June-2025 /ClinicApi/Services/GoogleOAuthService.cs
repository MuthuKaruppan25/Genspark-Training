
using Newtonsoft.Json.Linq;
using SecondWebApi.Interfaces;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IRepository<string, User> _repository;
    private readonly ITokenService _tokenService;

    public GoogleOAuthService(HttpClient httpClient, IConfiguration configuration, IRepository<string, User> repository, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _repository = repository;
        _tokenService = tokenService;
    }
    public async Task<string?> ExchangeCodeForTokenAsync(string code,string state)
    {
        var clientId = _configuration["GoogleOAuth:ClientId"];
        var clientSecret = _configuration["GoogleOAuth:ClientSecret"];
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"];

        var requestData = new Dictionary<string, string>
        {
            {"code", code},
            {"client_id", clientId!},
            {"client_secret", clientSecret!},
            {"redirect_uri", redirectUri!},
            {"grant_type", "authorization_code"}
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(requestData));

        if (!response.IsSuccessStatusCode)
            return null;

        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        var idToken = json["id_token"]?.ToString();

        if (idToken == null)
            return null;


        var tokenInfo = await _httpClient.GetStringAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
        var tokenPayload = JObject.Parse(tokenInfo);

        var email = tokenPayload["email"]?.ToString();

        var users = await _repository.GetAll();
        var user = users.FirstOrDefault(u => u.username == email);
        if (user is null)
        {
            user = await _repository.Add(new User { username = email!, role = state });
        }
        var token = await _tokenService.GenerateToken(user);
        return token;

    }

    public async Task<string?> GetGoogleLoginUrl(string role)
    {
        var clientId = _configuration["GoogleOAuth:ClientId"];
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
        var scope = "openid email profile";
        var responseType = "code";

        var state = Uri.EscapeDataString(role);

        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={clientId}" +
               $"&redirect_uri={redirectUri}" +
               $"&response_type={responseType}" +
               $"&scope={Uri.EscapeDataString(scope)}" +
               $"&state={state}";
    }


}