namespace GAC.WMS.Application.Common.IntegrationOptions
{
    public class ApplicationIntegrationOptions
    {
        public string? Url { get; set; }
        public string? Port { get; set; }
        public string? Username { get; set; } = "admin";
        public string? Password { get; set; } = "admin";
    }
}
