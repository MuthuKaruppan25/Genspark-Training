
namespace JobPortal.Models.DTOs;

public class RefreshTokenRequestDto
{
    public string ExpiredToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

}