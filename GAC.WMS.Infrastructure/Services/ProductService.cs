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
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<ProductDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<IProductService> _logger;
        public ProductService(AppDbContext dbContext, IMapper mapper, IValidatorService<ProductDto> validator,ILogger<IProductService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Product>()
                 .Select(p => _mapper.Map<ProductDto>(p))
                 .ToListAsync(cancellationToken);

        }

        public async Task<ProductDto?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<Product>()
                 .Where(p => p.Code == code)
                 .Select(c => _mapper.Map<ProductDto>(c))
                 .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(code);
            _logger.LogInformation($"Product with Code {code} found successfully.");
            return res;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<Product>()
                 .Where(p => p.Id == id)
                 .Select(c => _mapper.Map<ProductDto>(c))
                 .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
            _logger.LogInformation($"Product with ID {id} found successfully.");
            return res;
        }

        public async Task<ProductDto?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<Product>()
                .Where(p => p.Title == name)
                .Select(c => _mapper.Map<ProductDto>(c))
                .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(name);
            _logger.LogInformation($"Product with Name {name} found successfully.");
            return res;
        }
        public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<Product>(dto);
            _dbContext.Set<Product>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Product with ID {entity.Id} created successfully.");
            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<ProductDto> UpdateAsync(ProductDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Product>(dto);
            var existingEntity = await _dbContext.Set<Product>().FindAsync(new object[] { entity.Id }, cancellationToken);
            if (existingEntity == null)
                throw new ItemNotFoundException(dto.Id);

            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            _dbContext.Set<Product>().Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Product with ID {entity.Id} updated successfully.");
            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Set<Product>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<Product>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Product with ID {id} deleted successfully.");
            return true;
        }
    }
}
