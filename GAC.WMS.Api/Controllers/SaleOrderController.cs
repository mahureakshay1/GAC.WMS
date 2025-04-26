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
        public async Task<IEnumerable<SaleOrderDto>> Get(CancellationToken cancellationToken)
        {
            return await _sellOrderService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<SaleOrderDto?> Get(int id, CancellationToken cancellationToken)
        {
            return await _sellOrderService.GetByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<SaleOrderDto> Post([FromBody] SaleOrderDto dto, CancellationToken cancellationToken)
        {
            return await _sellOrderService.CreateAsync(dto, cancellationToken);
        }

        [HttpPut]
        public async Task<SaleOrderDto> Put(SaleOrderDto dto, CancellationToken cancellationToken)
        {
            return await _sellOrderService.UpdateAsync(dto, cancellationToken);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id, CancellationToken cancellationToken)
        {
            return await _sellOrderService.DeleteAsync(id, cancellationToken);
        }
    }
}
