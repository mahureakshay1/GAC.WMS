using AutoMapper;
using GAC.WMS.Application.Common;
using GAC.WMS.Application.Common.IntegrationModels;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces.External;
using Microsoft.Extensions.Logging;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
namespace GAC.WMS.Infrastructure.FileParsers
{
    public class XmlPurchaseOrderHandler : IIntegrationHandler
    {
        private readonly IMapper _mapper;
        private readonly IGacWmsClient _gacClient;
        private readonly ILogger<XmlPurchaseOrderHandler> _logger;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        public XmlPurchaseOrderHandler(IMapper mapper, IGacWmsClient gacClient, ILogger<XmlPurchaseOrderHandler> logger)
        {
            _mapper = mapper;
            _gacClient = gacClient;
            _logger = logger;
        }

        public bool CanHandleAsync(string filePath)
        {
            return filePath.EndsWith(".xml");
        }

        public async Task ProcessAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _gacClient.PostPurchaseOrderAsync(await ReadFileAsync(filePath));
                if (!result.IsSuccessStatusCode)
                    HandleError(filePath, await result.Content.ReadAsStringAsync(cancellationToken));
                else
                    HandleSuccess(filePath);
            }
            catch (Exception ex)
            {
                HandleError(filePath, ex.Message);
            }
        }

        private async Task<PurchaseOrderDto> ReadFileAsync(string filePath)
        {
            var xml = await File.ReadAllTextAsync(filePath);
            var serializer = new XmlSerializer(typeof(PurchaseOrderXmlModel));
            using var reader = new StringReader(xml);
            var model = serializer.Deserialize(reader) as PurchaseOrderXmlModel;
            if (model == null)
                HandleError(filePath, "Failed to deserialize the XML file into a PurchaseOrderXmlModel.");
            return _mapper.Map<PurchaseOrderDto>(model);
        }

        public void HandleError(string filePath, string errorMessage)
        {
            _lock.Wait();
            try
            {
                string fileDirectory = Path.GetDirectoryName(filePath) ??
                                        throw new InvalidCastException($"The directory for the file path '{filePath}' could not be determined.");
                string fileName = Path.GetFileName(filePath);
                string errorDir = Path.Combine(fileDirectory, "Error");
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(filePath);
                var comment = xmlDocument.CreateComment($"Error: {SecurityElement.Escape(errorMessage)}");
                xmlDocument.DocumentElement?.AppendChild(comment);
                xmlDocument.Save(filePath);

                if (!Directory.Exists(errorDir))
                {
                    Directory.CreateDirectory(errorDir);
                }
                File.Move(filePath, Path.Combine(errorDir, fileName));
                _logger.LogError(string.Format("File processing failed for file {0} :{1}", filePath, errorMessage));
            }
            finally
            {
                _lock.Release();
            }
        }

        public void HandleSuccess(string filePath)
        {
            _lock.Wait();
            try
            {
                string fileDirectory = Path.GetDirectoryName(filePath) ??
                                        throw new InvalidCastException($"The directory for the file path '{filePath}' could not be determined.");
                string fileName = Path.GetFileName(filePath);
                string archiveDir = Path.Combine(fileDirectory, "Success");
                if (!Directory.Exists(archiveDir))
                {
                    Directory.CreateDirectory(archiveDir);
                }
                File.Move(filePath, Path.Combine(archiveDir, fileName));
                _logger.LogInformation(string.Format("File processing done for file {0}", filePath));
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
