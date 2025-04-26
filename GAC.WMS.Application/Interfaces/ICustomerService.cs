using GAC.WMS.Application.Dtos;

namespace GAC.WMS.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CustomerDto?> GetByCompanyNameAsync(string name, CancellationToken cancellationToken);
        Task<CustomerDto> CreateAsync(CustomerDto dto, CancellationToken cancellationToken);
        Task<CustomerDto> UpdateAsync(CustomerDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
