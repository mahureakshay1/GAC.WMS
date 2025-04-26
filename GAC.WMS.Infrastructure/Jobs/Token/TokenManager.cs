using GAC.WMS.Application.Common.IntegrationOptions;
using GAC.WMS.Application.DTOs;
using GAC.WMS.Infrastructure.Integration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace GAC.WMS.Infrastructure.Jobs.Token
{
    public class TokenManager
    {
        private readonly TokenStore _tokenStore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JwtIntegrationOptions _jwtOptions;
        private readonly ApplicationIntegrationOptions _applicationIntegrationOptions;
        private readonly ILogger<GacWmsClient> _logger;
        const string LOGIN_FRA = "api/auth";
        const string GAC_CLIENT = "GacClient";
        public TokenManager(TokenStore tokenStore,
                            IHttpClientFactory httpClientFactory,
                            IOptions<ApplicationIntegrationOptions> applicationOptions,
                            IOptions<JwtIntegrationOptions> jwtOptions,
                            ILogger<GacWmsClient> logger)
        {
            _tokenStore = tokenStore;
            _httpClientFactory = httpClientFactory;
            _applicationIntegrationOptions = applicationOptions.Value;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;

        }
        private void SetToken(string token, DateTime validTill)
        {

            _tokenStore.SetToken(token, validTill);
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
        {
            if (_tokenStore.IsTokenValid())
            {
                return _tokenStore.GetToken();
            }
            else
            {
                var loginDto = new LoginDto()
                {
                    Username = _applicationIntegrationOptions.Username,
                    Password = _applicationIntegrationOptions.Password
                };
                var client = _httpClientFactory.CreateClient(GAC_CLIENT);

                var tokenResponse = await client.PostAsJsonAsync(LOGIN_FRA, loginDto, cancellationToken);
                if (tokenResponse.IsSuccessStatusCode)
                {
                    var token = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
                    var validTill = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes);
                    SetToken(token, validTill);
                    _logger.LogInformation("Token generated: " + token);
                }
                else
                {
                    _logger.LogError("Job authorization failed");
                    throw new UnauthorizedAccessException("Job authorization failed");
                }

            }
            return _tokenStore.GetToken();
        }
    }
}
