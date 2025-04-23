using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces.External
{
    public interface IGacWmsClient
    {
        Task<HttpResponseMessage> PostPurchaseOrderAsync(PurchaseOrderDto dto);
    }
}
