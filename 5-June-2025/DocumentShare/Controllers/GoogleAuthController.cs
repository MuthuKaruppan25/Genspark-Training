using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IGoogleOAuthService _googleOAuthService;

    public AuthController(IGoogleOAuthService googleOAuthService)
    {
        _googleOAuthService = googleOAuthService;
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLoginAsync(UserRegisterDto userRegisterDto)
    {
        var url =await _googleOAuthService.GetGoogleLoginUrl(userRegisterDto);
        return Ok(url);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code,[FromQuery]string state)
    {
        var jwt = await _googleOAuthService.ExchangeCodeForTokenAsync(code,state);

        if (jwt == null)
            return Unauthorized("Failed to authenticate with Google");

        return Ok(new { token = jwt });
    }
}
