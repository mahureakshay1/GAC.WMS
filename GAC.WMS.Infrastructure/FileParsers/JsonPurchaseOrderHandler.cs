using AutoMapper;
using GAC.WMS.Application.Common;
using GAC.WMS.Application.Interfaces.External;

namespace GAC.WMS.Infrastructure.FileParsers
{
    public class JsonPurchaseOrderHandler : IIntegrationHandler
    {
        private readonly IMapper _mapper;
        private readonly IGacWmsClient _gacClient;

        public JsonPurchaseOrderHandler(IMapper mapper, IGacWmsClient gacClient)
        {
            _mapper = mapper;
            _gacClient = gacClient;
        }

        public bool CanHandleAsync(string filePath)
        {
            return filePath.EndsWith(".json");
        }

        public void HandleError(string filePath, string errorMessage)
        {
            throw new NotImplementedException();
        }

        public void HandleSuccess(string filePath)
        {
            throw new NotImplementedException();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ProcessAsync(string filePath, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            throw new NotImplementedException("JSON file processing is not implemented yet.");
        }
    }
}
