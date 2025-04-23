using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.DTOs;
using GAC.WMS.Application.Interfaces.External;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GAC.WMS.Infrastructure.Integration
{
    public class GacWmsClient : IGacWmsClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GacWmsClient> _logger;
        private readonly IConfiguration _configuration;

        const string LOGIN_FRA = "api/auth";
        const string ORDER_API_FRA = "api/purchaseorder";
        const string LOGIN_CLIENT = "GacAuth";
        const string GAC_CLIENT = "GacClient";
        const string USERNAME = "admin";
        const string PASSWORD = "admin";

        public GacWmsClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GacWmsClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;

        }
        public async Task<HttpResponseMessage> PostPurchaseOrderAsync(PurchaseOrderDto dto)
        {
            CancellationToken cancellationToken = new CancellationToken();
            var loginDto = new LoginDto()
            {
                Username = _configuration["Application:Username"] ?? USERNAME,
                Password = _configuration["Application:Password"] ?? PASSWORD
            };

            var loginClinet = _httpClientFactory.CreateClient(LOGIN_CLIENT);

            var tokenResponse = await loginClinet.PostAsJsonAsync(LOGIN_FRA, loginDto, cancellationToken);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var token = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
                var gacClient = _httpClientFactory.CreateClient(GAC_CLIENT);
                gacClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.Trim('"'));
                _logger.LogInformation("Token generated for job purchase order object: " + JsonSerializer.Serialize(dto));
                return await gacClient.PostAsJsonAsync(ORDER_API_FRA, dto, cancellationToken);
            }
            else
            {
                _logger.LogError("Job authorization failed for object: " + JsonSerializer.Serialize(dto));
                throw new UnauthorizedAccessException("Job authorization failed");
            }
        }
    }
}