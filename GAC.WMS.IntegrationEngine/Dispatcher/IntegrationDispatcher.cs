using GAC.WMS.Application.Common;

namespace GAC.WMS.IntegrationEngine.Dispatcher
{
    public class IntegrationDispatcher
    {
        private readonly IEnumerable<IIntegrationHandler> _handlers;

        public IntegrationDispatcher(IEnumerable<IIntegrationHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task DispatchAsync(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            foreach (var file in files)
            {
                var handler = _handlers.FirstOrDefault(h => h.CanHandleAsync(file));
                if (handler != null)
                {
                    await handler.ProcessAsync(file);
                }
            }
        }
    }
}
