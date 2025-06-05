

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentShare.Interfaces;
using DocumentShare.Models;
using Microsoft.IdentityModel.Tokens;


public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey;
    private readonly IRepository<string, User> _repository;


    public TokenService(IConfiguration configuration, IRepository<string, User> repository)
    {
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));

        _repository = repository;
    }
    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Username),
                                new Claim(ClaimTypes.Role, user.Role)
                            };


        var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(10),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
