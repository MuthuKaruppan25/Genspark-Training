
using DocumentShare.Interfaces;
using DocumentShare.Models;
using Newtonsoft.Json.Linq;


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
    public async Task<string?> GetGoogleLoginUrl(UserRegisterDto userRegisterDto)
    {
        var clientId = _configuration["GoogleOAuth:ClientId"];
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
        var scope = "openid email profile";
        var responseType = "code";

        var state = EncodeState(userRegisterDto);

        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={clientId}" +
               $"&redirect_uri={redirectUri}" +
               $"&response_type={responseType}" +
               $"&scope={Uri.EscapeDataString(scope)}" +
               $"&state={state}";
    }
    public async Task<string?> ExchangeCodeForTokenAsync(string code, string state)
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

      
        var decodedStateJson = Uri.UnescapeDataString(state);
        var userRegisterDto = System.Text.Json.JsonSerializer.Deserialize<UserRegisterDto>(decodedStateJson);
        var role = userRegisterDto?.Role ?? "User"; 

        var tokenInfo = await _httpClient.GetStringAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
        var tokenPayload = JObject.Parse(tokenInfo);
        var email = tokenPayload["email"]?.ToString();

        if (email == null)
            return null;

        
        var users = await _repository.GetAll();
        var user = users.FirstOrDefault(u => u.Username == email);

        
        if (user is null)
        {
        
            user = await _repository.Add(new User
            {
                Username = email,
                Role = role,
                Name = userRegisterDto?.Name ?? "GoogleUser",
                Department = userRegisterDto!.Department,
                PasswordHash = Array.Empty<byte>(),
                HashKey =Array.Empty<byte>()
            });
        }

        var token = await _tokenService.GenerateToken(user);
        return token;
    }



    public string EncodeState(UserRegisterDto dto)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(dto);
        return Uri.EscapeDataString(json);
    }


}