using GAC.WMS.Application.Common;
using GAC.WMS.Application.Common.IntegrationOptions;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Application.Interfaces.External;
using GAC.WMS.Infrastructure.FileParsers;
using GAC.WMS.Infrastructure.Integration;
using GAC.WMS.Infrastructure.Persistence;
using GAC.WMS.Infrastructure.Services;
using GAC.WMS.IntegrationEngine.Dispatcher;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace GAC.WMS.Infrastructure.ServiceExtension
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Add configurations 
            services.Configure<JwtIntegrationOptions>(configuration.GetSection(typeof(JwtIntegrationOptions).Name));
            services.Configure<ApplicationIntegrationOptions>(configuration.GetSection(typeof(ApplicationIntegrationOptions).Name));
            services.Configure<FileIntegrationOptions>(configuration.GetSection(typeof(FileIntegrationOptions).Name));
            services.Configure<RateLimiterIntegrationOptions>(configuration.GetSection(typeof(RateLimiterIntegrationOptions).Name));

            // add automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // db context
            services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
             );

            // log service
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            // application services
            services.AddScoped(typeof(IValidatorService<>), typeof(ValidatorService<>));
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ISaleOrderService, SaleOrderService>();

            // job 
            services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddScoped<IntegrationDispatcher>();
            services.AddScoped<IIntegrationHandler, XmlPurchaseOrderHandler>();
            services.AddScoped<IIntegrationHandler, CsvPurchaseOrderHandler>();
            services.AddScoped<IIntegrationHandler, JsonPurchaseOrderHandler>();
            services.AddTransient<IGacWmsClient, GacWmsClient>();

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var jwtIntegrationOptions = scope.ServiceProvider.GetRequiredService<IOptions<JwtIntegrationOptions>>().Value;
                var applicationIntegrationOptions = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationIntegrationOptions>>().Value;
                var rateIntegrationOptions = scope.ServiceProvider.GetRequiredService<IOptions<RateLimiterIntegrationOptions>>().Value;
                // jwt configuration
                services.AddJwtServices(jwtIntegrationOptions);

                // Rate limiter 
                services.AddRateLimiterServices(rateIntegrationOptions);

                // Add application client
                services.AddHttpClient("GacClient", client =>
                {
                    client.BaseAddress = new Uri(applicationIntegrationOptions.Url + applicationIntegrationOptions.Port);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                });
            }
            return services;
        }
    }
}
