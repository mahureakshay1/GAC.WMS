using GAC.WMS.Application.Common.IntegrationOptions;
using GAC.WMS.Infrastructure.Jobs.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GAC.WMS.Infrastructure.ServiceExtension
{
    public static class JwtServiceExtension
    {
        public static IServiceCollection AddJwtServices(this IServiceCollection services, JwtIntegrationOptions jwtIntegrationOptions)
        {
            services.AddSingleton<TokenStore>();
            services.AddScoped<TokenManager>();

            var jwtKey = Encoding.UTF8.GetBytes(jwtIntegrationOptions.Secret ?? throw new InvalidOperationException("Security key is missing"));
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
                    ValidIssuer = jwtIntegrationOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtIntegrationOptions.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                };
                
            });
            services.AddAuthorization(options =>
            {
                // Default policy uses JwtBearer
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}
