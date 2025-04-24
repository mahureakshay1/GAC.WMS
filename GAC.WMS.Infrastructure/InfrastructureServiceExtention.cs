using GAC.WMS.Application.Common;
using GAC.WMS.Application.Common.IntegrationModels;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Application.Interfaces.External;
using GAC.WMS.Infrastructure.FileParsers;
using GAC.WMS.Infrastructure.Integration;
using GAC.WMS.Infrastructure.Persistence;
using GAC.WMS.Infrastructure.Services;
using GAC.WMS.IntegrationEngine.Dispatcher;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace GAC.WMS.Infrastructure
{
    public static class InfrastructureServiceExtention
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // db context
            services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
             );

            // jwt configuration
            var jwtKey = Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
                    {
                        jwtOptions.RequireHttpsMetadata = false;
                        jwtOptions.SaveToken = true;
                        jwtOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings["Issuer"],

                            ValidateAudience = true,
                            ValidAudience = jwtSettings["Audience"],

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                        };
                        jwtOptions.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                context.Response.StatusCode = 401;
                                Console.WriteLine($"JWT Auth failed: {context.Exception.Message}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine("JWT Token validated successfully.");
                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                Console.WriteLine("Token received.");
                                return Task.CompletedTask;
                            },
                            OnChallenge = context =>
                            {
                                Console.WriteLine("JWT Challenge triggered.");
                                return Task.CompletedTask;
                            }
                        };

                    });
            services.AddAuthorization(options =>
            {
                // Default policy uses JwtBearer
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

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

            var fileIntegrationOptions = configuration.GetSection("FileIntegration").Get<FileIntegrationOptions>();

            // job 
            if (fileIntegrationOptions == null)
                throw new InvalidOperationException("FileIntegrationOptions configuration section is missing or invalid.");
            services.AddSingleton(fileIntegrationOptions);
            services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddScoped<IIntegrationHandler, XmlPurchaseOrderHandler>();
            services.AddScoped<IntegrationDispatcher>();
            services.AddSingleton<IIntegrationHandler, XmlPurchaseOrderHandler>();
            services.AddSingleton<IIntegrationHandler, CsvPurchaseOrderHandler>();
            services.AddSingleton<IIntegrationHandler, JsonPurchaseOrderHandler>();
            services.AddTransient<IGacWmsClient, GacWmsClient>();
            services.AddHttpClient("GacClient", client =>
            {
                client.BaseAddress = new Uri(configuration["Application:Url"] + configuration["Application:Port"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("GacAuth", client =>
            {
                client.BaseAddress = new Uri(configuration["Application:Url"] + configuration["Application:Port"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Rate limiter 
            services.AddRateLimiter(options =>
            {
                options.AddTokenBucketLimiter("tokenbucket", limiterOptions =>
                {
                    limiterOptions.TokenLimit = 10;
                    limiterOptions.TokensPerPeriod = 1;
                    limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(2);
                    limiterOptions.QueueLimit = 10;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.AutoReplenishment = true;
                });
            });

            return services;
        }
    }
}
