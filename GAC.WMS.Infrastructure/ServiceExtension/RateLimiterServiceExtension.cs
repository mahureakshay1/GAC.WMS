using GAC.WMS.Application.Common.IntegrationOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.RateLimiting;

namespace GAC.WMS.Infrastructure.ServiceExtension
{
    public static class RateLimiterServiceExtension
    {
        public static IServiceCollection AddRateLimiterServices(this IServiceCollection services, RateLimiterIntegrationOptions rateIntegrationOptions)
        {
            services.AddRateLimiter(options =>
            {
                options.AddTokenBucketLimiter("tokenbucket", limiterOptions =>
                {
                    limiterOptions.TokenLimit = rateIntegrationOptions.TokenLimit;
                    limiterOptions.TokensPerPeriod = rateIntegrationOptions.TokensPerPeriod;
                    limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(rateIntegrationOptions.ReplenishmentPeriod);
                    limiterOptions.QueueLimit = rateIntegrationOptions.QueueLimit;
                    limiterOptions.QueueProcessingOrder = rateIntegrationOptions.QueueProcessingOrder.Equals("FIFO") ?
                                        QueueProcessingOrder.OldestFirst : QueueProcessingOrder.NewestFirst;
                    limiterOptions.AutoReplenishment = rateIntegrationOptions.AutoReplenishment;
                });
                options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
            });

            return services;
        }
    }
}
