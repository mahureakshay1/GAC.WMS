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
    public class SaleOrderService : ISaleOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly IValidatorService<SaleOrderDto> _orderValidator;
        private readonly IValidatorService<SaleOrderLineDto> _orderLineValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ISaleOrderService> _logger;
        public SaleOrderService(AppDbContext dbContext, IMapper mapper, IValidatorService<SaleOrderDto> validator, IValidatorService<SaleOrderLineDto> orderLineValidator, ILogger<ISaleOrderService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _orderValidator = validator;
            _orderLineValidator = orderLineValidator;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleOrderDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Set<SaleOrder>()
               .Include(c => c.SaleOrderLines)
               .Select(c => _mapper.Map<SaleOrderDto>(c))
               .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SaleOrderDto>> GetByCustomerNameAsync(string name, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
               .Where(c => c.Customer.CompanyName == name)
               .Select(c => _mapper.Map<SaleOrderDto>(c))
               .ToListAsync(cancellationToken);
            if (res.Count == 0)
                throw new ItemNotFoundException(name);
            _logger.LogInformation($"SaleOrder with Customer Name {name} found successfully.");
            return res;
        }

        public async Task<SaleOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var res = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
                 .Where(c => c.Id == id)
                 .Select(c => _mapper.Map<SaleOrderDto>(c))
                 .FirstOrDefaultAsync(cancellationToken);
            if (res == null)
                throw new ItemNotFoundException(id);
            _logger.LogInformation($"SaleOrder with ID {id} found successfully.");
            return res;
        }

        public async Task<SaleOrderDto> CreateAsync(SaleOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderValidator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<SaleOrder>(dto);
            _dbContext.Set<SaleOrder>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"SaleOrder with ID {entity.Id} created successfully.");
            return _mapper.Map<SaleOrderDto>(entity);
        }

        public async Task<SaleOrderDto> UpdateAsync(SaleOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderLineValidator.ValidateAsync(dto.SaleOrderLines, cancellationToken);
            var entity = _mapper.Map<SaleOrder>(dto);

            var existingEntity = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
                .FirstOrDefaultAsync(c => c.Id == dto.Id, cancellationToken);

            if (existingEntity == null)
                throw new ItemNotFoundException(dto.Id);

            var newSaleOrderLine = new List<SaleOrderLine>();

            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            if (entity.SaleOrderLines != null)
                foreach (var line in entity.SaleOrderLines)
                {
                    var existingLine = existingEntity.SaleOrderLines?.FirstOrDefault(l => l.Id == line.Id);
                    if (existingLine != null)
                        _dbContext.Entry(existingLine).CurrentValues.SetValues(line);
                    else
                        newSaleOrderLine.Add(line);
                }

            if (newSaleOrderLine.Count() > 0)
                _dbContext.Set<SaleOrderLine>().AddRange(newSaleOrderLine);

            if (existingEntity.SaleOrderLines != null)
                foreach (var line in existingEntity.SaleOrderLines)
                {
                    if (entity.SaleOrderLines != null && !entity.SaleOrderLines.Any(l => l.Id == line.Id))
                        _dbContext.Set<SaleOrderLine>().Remove(line);
                }
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"SaleOrder with ID {dto.Id} updated successfully.");
            return _mapper.Map<SaleOrderDto>(existingEntity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            SaleOrder? entity = _dbContext.Set<SaleOrder>().Find(id);
            if (entity == null)
                throw new ItemNotFoundException(id);
            _dbContext.Set<SaleOrder>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"SaleOrder with ID {id} deleted successfully.");
            return true;
        }
    }
}
