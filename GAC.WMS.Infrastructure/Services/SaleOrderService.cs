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
        private readonly IValidatorService<SellOrderDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<ISaleOrderService> _logger;
        public SaleOrderService(AppDbContext dbContext, IMapper mapper, IValidatorService<SellOrderDto> validator, ILogger<ISaleOrderService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
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
            _logger.LogInformation($"SaleOrder with Customer Name {name} found successfully.");
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
            _logger.LogInformation($"SaleOrder with ID {id} found successfully.");
            return res;
        }

        public async Task<SellOrderDto> CreateAsync(SellOrderDto dto, CancellationToken cancellationToken)
        {
            await _validator.ValidateAsync(dto, cancellationToken);
            var entity = _mapper.Map<SaleOrder>(dto);
            _dbContext.Set<SaleOrder>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"SaleOrder with ID {entity.Id} created successfully.");
            return _mapper.Map<SellOrderDto>(entity);
        }

        public async Task<SellOrderDto> UpdateAsync(SellOrderDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SaleOrder>(dto);
            var existingEntity = await _dbContext.Set<SaleOrder>()
                .Include(c => c.SaleOrderLines)
                .FirstOrDefaultAsync(c => c.Id == dto.Id, cancellationToken);
            if (existingEntity == null)
                throw new ItemNotFoundException(dto.Id);

            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            if (entity.SaleOrderLines != null)
                foreach (var line in entity.SaleOrderLines)
                {
                    var existingLine = existingEntity.SaleOrderLines?.FirstOrDefault(l => l.Id == line.Id);
                    if (existingLine != null)
                    {
                        _dbContext.Entry(existingLine).CurrentValues.SetValues(line);
                    }
                    else
                    {
                        existingEntity.SaleOrderLines?.Add(line);
                    }
                }
            if (existingEntity.SaleOrderLines != null)
                foreach (var line in existingEntity.SaleOrderLines)
                {
                    if (entity.SaleOrderLines !=null && !entity.SaleOrderLines.Any(l => l.Id == line.Id))
                    {
                        _dbContext.Set<SaleOrderLine>().Remove(line);
                    }
                }
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"SaleOrder with ID {dto.Id} updated successfully.");
            return _mapper.Map<SellOrderDto>(existingEntity);
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
