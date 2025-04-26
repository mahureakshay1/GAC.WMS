namespace GAC.WMS.Application.Common.IntegrationOptions
{
    public class JwtIntegrationOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Secret { get; set; }
        public int ExpirationInMinutes { get; set; } = 60;
        public bool ValidateIssuerSigningKey { get; set; } = true;
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
        public bool RequireExpirationTime { get; set; } = true;
    }
}
