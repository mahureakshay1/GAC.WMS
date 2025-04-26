using GAC.WMS.Application.Common.IntegrationOptions;
using GAC.WMS.Application.DTOs;
using GAC.WMS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GAC.WMS.Infrastructure.Services
{
    /// <summary>
    /// Jwt auth service checking hardcoded username and password
    /// This must read from database or other secure storage
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly JwtIntegrationOptions _jwtIntegrationOptions;
        private readonly ApplicationIntegrationOptions _applicationIntegrationOptions;
        private readonly ILogger<IAuthService> _logger;

        public AuthService(IOptions<JwtIntegrationOptions> jwtOptions, IOptions<ApplicationIntegrationOptions> applicationOptions, ILogger<IAuthService> logger)
        {
            _jwtIntegrationOptions = jwtOptions.Value;
            _applicationIntegrationOptions = applicationOptions.Value;
            _logger = logger;
        }
        public async Task<string> AuthenticateAsync(LoginDto loginModel, CancellationToken cancellationToken)
        {
            if (loginModel.Username == _applicationIntegrationOptions.Username &&
                loginModel.Password == _applicationIntegrationOptions.Password)
                return await Task.Run(() => GenerateJwtToken());

            throw new UnauthorizedAccessException();
        }

        private string GenerateJwtToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtIntegrationOptions.Secret ??
                                    throw new InvalidOperationException("JWT secret key is not configured.")));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtIntegrationOptions.Issuer,
                audience: _jwtIntegrationOptions.Audience,
                claims: new List<Claim>
                {
                     new Claim(ClaimTypes.Name, "admin"),
                     new Claim(ClaimTypes.Role, "admin")
                },
                expires: DateTime.UtcNow.AddMinutes(_jwtIntegrationOptions.ExpirationInMinutes),
                signingCredentials: signingCredentials
            );
            _logger.LogInformation(string.Format("Token generated"));
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
