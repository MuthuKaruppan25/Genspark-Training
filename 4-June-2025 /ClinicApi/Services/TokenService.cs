

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey;
    private readonly IRepository<int, Patient> _patientRepository;
    private readonly IRepository<int, Doctor> _doctorRepository;

    public TokenService(IConfiguration configuration, IRepository<int, Patient> patientRepository, IRepository<int, Doctor> doctorRepository)
    {
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;

    }
    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.username),
        new Claim(ClaimTypes.Role, user.role)
    };


        if (user.role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
        {
            var allDoctors = await _doctorRepository.GetAll();
            var doctor = allDoctors.FirstOrDefault(d => d.Email.Equals(user.username, StringComparison.OrdinalIgnoreCase));

            if (doctor != null)
            {
                claims.Add(new Claim("YearsOfExperience", doctor.YearsOfExperience.ToString()));
            }
        }

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
