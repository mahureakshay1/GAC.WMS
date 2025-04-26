using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GAC.WMS.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<CustomerDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<ICustomerService> _logger;
        public CustomerService(AppDbContext dbContext, IMapper mapper, IValidatorService<CustomerDto> validator, ILogger<ICustomerService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }
        public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Customer>()
                 .Select(c => _mapper.Map<CustomerDto>(c))
                 .ToListAsync(cancellationToken);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<Customer>()
                .Where(c => c.Id == id)
                .Select(c => _mapper.Map<CustomerDto>(c))
                .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
            _logger.LogInformation($"Customer with ID {id} found successfully.");
            return res;
        }

        public async Task<CustomerDto?> GetByCompanyNameAsync(string name, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<Customer>()
                .Where(c => c.CompanyName == name)
                .Select(c => _mapper.Map<CustomerDto>(c))
                .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(name);
            _logger.LogInformation($"Customer with Company Name {name} found successfully.");
            return res;
        }

        public async Task<CustomerDto> CreateAsync(CustomerDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<Customer>(dto);
            _dbContext.Set<Customer>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Customer with ID {entity.Id} created successfully.");
            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<CustomerDto> UpdateAsync(CustomerDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Customer>(dto);
            var existingEntity = await _dbContext.Set<Customer>().FindAsync(new object[] { dto.Id }, cancellationToken);
            if (existingEntity == null)
                throw new ItemNotFoundException(dto.Id);
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Customer with ID {dto.Id} updated successfully.");
            return _mapper.Map<CustomerDto>(existingEntity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Set<Customer>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<Customer>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Customer with ID {id} deleted successfully.");

            return true;
        }
    }
}
