using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (loginRequestDto == null)
                return BadRequest("Login data is required.");

            try
            {
                var response = await _authenticationService.LoginAsync(loginRequestDto);
                return Ok(response);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (refreshTokenRequestDto == null)
                return BadRequest("Refresh token data is required.");

            try
            {
                var newAccessToken = await _authenticationService.RefreshTokenAsync(
                    refreshTokenRequestDto.ExpiredToken,
                    refreshTokenRequestDto.RefreshToken
                );
                return Ok(new { AccessToken = newAccessToken });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
        {
            if (logoutRequestDto == null || string.IsNullOrWhiteSpace(logoutRequestDto.username))
                return BadRequest("Username is required for logout.");

            try
            {
                await _authenticationService.LogoutAsync(logoutRequestDto.username);
                return Ok(new { message = "Logout successful." });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}