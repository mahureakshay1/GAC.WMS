using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<PurchaseOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken);
        Task<PurchaseOrderDto> CreateAsync(PurchaseOrderDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
