

using System.Security.Claims;
using JobPortal.Models;
namespace JobPortal.Interfaces;
public interface ITokenService
{
    Task<string> GenerateAccessToken(User user);
    Task<string> GenerateRefreshToken(User user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}