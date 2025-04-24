using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<ProductDto> _validator;
        private readonly IMapper _mapper;
        public ProductService(AppDbContext dbContext, IMapper mapper, IValidatorService<ProductDto> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
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
            return res;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res= await _dbContext.Set<Product>()
                 .Where(p => p.Id == id)
                 .Select(c => _mapper.Map<ProductDto>(c))
                 .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
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
            return res;
        }
        public async Task<ProductDto> CreateAsync(ProductDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<Product>(dto);
            _dbContext.Set<Product>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Set<Product>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<Product>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
