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
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<PurchaseOrderDto> _orderValidator;
        private readonly IValidatorService<PurchaseOrderLineDto> _orderLineValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<IPurchaseOrderService> _logger;
        public PurchaseOrderService(AppDbContext dbContext, IMapper mapper, IValidatorService<PurchaseOrderDto> orderValidator, IValidatorService<PurchaseOrderLineDto> orderLineValidator, ILogger<IPurchaseOrderService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _orderValidator = orderValidator;
            _orderLineValidator = orderLineValidator;
            _logger = logger;
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
            _logger.LogInformation($"PurchaseOrder with ID {id} found successfully.");
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
            _logger.LogInformation($"PurchaseOrder with Customer Name {name} found successfully.");
            return res;
        }
        public async Task<PurchaseOrderDto> CreateAsync(PurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderValidator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<PurchaseOrder>(dto);
            _dbContext.Set<PurchaseOrder>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"PurchaseOrder with ID {entity.Id} created successfully.");
            return _mapper.Map<PurchaseOrderDto>(entity);
        }

        public async Task<PurchaseOrderDto> UpdateAsync(PurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderLineValidator.ValidateAsync(dto.PurchaseOrderLines, cancellationToken);
            var entity = _mapper.Map<PurchaseOrder>(dto);

            var existingEntity = await _dbContext.Set<PurchaseOrder>()
                .Include(SaleOrderDto => SaleOrderDto.PurchaseOrderLines)
                .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (existingEntity == null)
                throw new ItemNotFoundException(dto.Id);

            var newPurchaseOrderLine = new List<PurchaseOrderLine>();

            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            if (entity.PurchaseOrderLines != null)
                foreach (var line in entity.PurchaseOrderLines)
                {
                    var existingLine = existingEntity.PurchaseOrderLines?.FirstOrDefault(l => l.Id == line.Id);
                    if (existingLine != null)
                        _dbContext.Entry(existingLine).CurrentValues.SetValues(line);
                    else
                        newPurchaseOrderLine.Add(line);
                }

            if (newPurchaseOrderLine.Count > 0)
                existingEntity.PurchaseOrderLines?.AddRange(newPurchaseOrderLine);

            if (existingEntity.PurchaseOrderLines != null)
                foreach (var line in existingEntity.PurchaseOrderLines)
                {
                    if (entity.PurchaseOrderLines != null && !entity.PurchaseOrderLines.Any(l => l.Id == line.Id))
                    {
                        _dbContext.Set<PurchaseOrderLine>().Remove(line);
                    }
                }
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"PurchaseOrder with ID {entity.Id} updated successfully.");
            return _mapper.Map<PurchaseOrderDto>(existingEntity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.Set<PurchaseOrder>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<PurchaseOrder>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"PurchaseOrder with ID {id} deleted successfully.");
            return true;
        }
    }
}
