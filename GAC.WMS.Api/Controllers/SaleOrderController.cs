using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleOrderController : ControllerBase
    {

        private readonly ISaleOrderService _sellOrderService;
        public SaleOrderController(ISaleOrderService sellOrderService)
        {
            _sellOrderService = sellOrderService;
        }

        [HttpGet]
        public async Task<IEnumerable<SellOrderDto>> Get(CancellationToken cancellationToken)
        {
            return await _sellOrderService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<SellOrderDto?> Get(int id, CancellationToken cancellationToken)
        {
            return await _sellOrderService.GetByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<SellOrderDto> Post([FromBody] SellOrderDto dto, CancellationToken cancellationToken)
        {
            return await _sellOrderService.CreateAsync(dto, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<SellOrderDto> Put(SellOrderDto dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id, CancellationToken cancellationToken)
        {
            return await _sellOrderService.DeleteAsync(id, cancellationToken);
        }
    }
}
