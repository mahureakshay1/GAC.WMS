using GAC.WMS.Application.DTOs;
using GAC.WMS.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace GAC.WMS.Infrastructure.Services
{
    /// <summary>
    /// Basic auth service checking hardcoded username and password
    /// This must read from database or other secure storage
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Authenticate(LoginDto loginModel, CancellationToken cancellationToken)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "admin")
                return GenerateJwtToken();

            throw new UnauthorizedAccessException();
        }

        private string GenerateJwtToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: new List<Claim>
                {
                     new Claim(ClaimTypes.Name, "admin"),
                     new Claim(ClaimTypes.Role, "admin")
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
