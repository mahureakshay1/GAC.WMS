namespace GAC.WMS.Application.Common.IntegrationModels
{
    public class CustomerFileIntegrationConfig
    {
        public required string Name { get; set; }
        public required string Path { get; set; }
        public string? Type { get; set; }
        
    }

    public class FileIntegrationOptions
    {
        public required string CronExpression { get; set; }
        public required List<CustomerFileIntegrationConfig> Customers { get; set; }
    }
}
