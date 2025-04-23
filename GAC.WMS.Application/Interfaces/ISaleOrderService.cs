using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces
{
    public interface ISaleOrderService
    {
        Task<IEnumerable<SellOrderDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<SellOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<SellOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken);
        Task<SellOrderDto> CreateAsync(SellOrderDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
