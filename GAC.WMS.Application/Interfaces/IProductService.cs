using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductDto?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<ProductDto?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
