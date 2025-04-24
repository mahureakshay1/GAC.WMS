using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Services
{
    public class SaleOrderService : ISaleOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<SellOrderDto> _validator;
        private readonly IMapper _mapper;
        public SaleOrderService(AppDbContext dbContext, IMapper mapper, IValidatorService<SellOrderDto> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<SellOrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<SaleOrder>()
               .Include(c => c.SaleOrderLines)
               .Select(c => _mapper.Map<SellOrderDto>(c))
               .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SellOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
               .Where(c => c.Customer.CompanyName == name)
               .Select(c => _mapper.Map<SellOrderDto>(c))
               .ToListAsync(cancellationToken);
            if (res.Count == 0)
                throw new ItemNotFoundException(name);
            return res;
        }

        public async Task<SellOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
                 .Where(c => c.Id == id)
                 .Select(c => _mapper.Map<SellOrderDto>(c))
                 .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
            return res;
        }

        public async Task<SellOrderDto> CreateAsync(SellOrderDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<SaleOrder>(dto);
            _dbContext.Set<SaleOrder>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<SellOrderDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            SaleOrder? entity = _dbContext.Set<SaleOrder>().Find(id);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<SaleOrder>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
