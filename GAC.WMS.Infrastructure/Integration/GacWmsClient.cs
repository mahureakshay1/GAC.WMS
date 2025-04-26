using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces.External;
using GAC.WMS.Infrastructure.Jobs.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly TokenManager _tokenManager;
        const string ORDER_API_FRA = "api/purchaseorder";
        const string GAC_CLIENT = "GacClient";



        public GacWmsClient(IHttpClientFactory httpClientFactory, TokenManager tokenManager, ILogger<GacWmsClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _logger = logger;

        }
        public async Task<HttpResponseMessage> PostPurchaseOrderAsync(PurchaseOrderDto dto)
        {
            CancellationToken cancellationToken = new CancellationToken();

            var token = await _tokenManager.GetTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                var gacClient = _httpClientFactory.CreateClient(GAC_CLIENT);
                gacClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.Trim('"'));
                _logger.LogInformation("Job: Calling API " + ORDER_API_FRA + " for order object: " + JsonSerializer.Serialize(dto));
                return await gacClient.PostAsJsonAsync(ORDER_API_FRA, dto, cancellationToken);
            }
            else
            {
                _logger.LogError("Job: Api call failed for object: " + JsonSerializer.Serialize(dto));
                throw new InvalidOperationException("Job:  API call failed " + ORDER_API_FRA + " for order object: " + JsonSerializer.Serialize(dto));
            }
        }
    }
}