using GAC.WMS.Application.Common.IntegrationModels;
using GAC.WMS.IntegrationEngine.Dispatcher;
using Hangfire;

namespace GAC.WMS.Infrastructure.Job
{
    public class PurchaseOrderImporter
    {
        private readonly IntegrationDispatcher _dispatcher;
        private readonly FileIntegrationOptions _fileOptions;
        public PurchaseOrderImporter(IntegrationDispatcher dispatcher, FileIntegrationOptions fileOptions)
        {
            _dispatcher = dispatcher;
            _fileOptions = fileOptions;
        }

        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteAsync()
        {
            foreach (var customer in _fileOptions.Customers)
            {
                await _dispatcher.DispatchAsync(customer.Path);
            }
        }
    }
}