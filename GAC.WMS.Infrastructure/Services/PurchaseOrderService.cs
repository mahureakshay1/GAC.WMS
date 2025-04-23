using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GAC.WMS.Infrastructure.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<PurchaseOrderDto> _validator;
        private readonly IMapper _mapper;
        public PurchaseOrderService(AppDbContext dbContext, IMapper mapper, IValidatorService<PurchaseOrderDto> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }
        public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<PurchaseOrder>()
                 .Include(c => c.PurchaseOrderLines)
                 .Select(c => _mapper.Map<PurchaseOrderDto>(c))
                 .ToListAsync(cancellationToken);
        }
        public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<PurchaseOrder>()
                .Where(c => c.Id == id)
                .Include(c => c.PurchaseOrderLines)
                .Select(c => _mapper.Map<PurchaseOrderDto>(c))
                .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
            return res;
        }
        public async Task<IEnumerable<PurchaseOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<PurchaseOrder>()
                .Include(c => c.PurchaseOrderLines)
                .Where(c => c.Customer.CompanyName == name)
                .Select(c => _mapper.Map<PurchaseOrderDto>(c))
                .ToListAsync(cancellationToken);
            if (res.Count == 0)
                throw new ItemNotFoundException(name);
            return res;
        }
        public async Task<PurchaseOrderDto> CreateAsync(PurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<PurchaseOrder>(dto);
            _dbContext.Set<PurchaseOrder>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PurchaseOrderDto>(entity);
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Set<PurchaseOrder>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<PurchaseOrder>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
