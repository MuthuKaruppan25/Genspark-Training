using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SecondWebApi.Interfaces;
using SecondWebApi.Misc;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly Interfaces.IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }
    [HttpPost]
    [CustomExceptionFilter]
    public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
    {
        // try
        // {
        //     var result = await _authenticationService.AuthenticateUser(loginRequest);
        //     return Ok(result);
        // }
        // catch (Exception e)
        // {
        //     _logger.LogError(e.Message);
        //     return Unauthorized(e.Message);
        // }
        var result = await _authenticationService.AuthenticateUser(loginRequest);
        return Ok(result);
    }
}