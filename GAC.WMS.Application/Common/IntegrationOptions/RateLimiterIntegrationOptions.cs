namespace GAC.WMS.Application.Common.IntegrationOptions
{
    public class RateLimiterIntegrationOptions
    {
        public int TokenLimit { get; set; } = 10;
        public int TokensPerPeriod { get; set; } = 1;
        public int ReplenishmentPeriod { get; set; } = 1;
        public int QueueLimit { get; set; } = 10;
        public string QueueProcessingOrder { get; set; } = "FIFO";
        public bool AutoReplenishment { get; set; } = true;
    }
}
