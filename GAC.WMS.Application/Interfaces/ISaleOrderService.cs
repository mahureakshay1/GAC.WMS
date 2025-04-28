using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces
{
    public interface ISaleOrderService
    {
        Task<IEnumerable<SaleOrderDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<SaleOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<SaleOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken);
        Task<SaleOrderDto> CreateAsync(SaleOrderDto dto, CancellationToken cancellationToken);
        Task<SaleOrderDto> UpdateAsync(SaleOrderDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
