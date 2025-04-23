using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<IEnumerable<PurchaseOrderDto>> Get(CancellationToken cancellationToken)
        {
            return await _purchaseOrderService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<PurchaseOrderDto?> Get(int id, CancellationToken cancellationToken)
        {
            return await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<PurchaseOrderDto> Post([FromBody] PurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            return await _purchaseOrderService.CreateAsync(dto, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<PurchaseOrderDto> Put(int id, [FromBody] PurchaseOrderDto dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _purchaseOrderService.DeleteAsync(id, CancellationToken.None);
        }
    }
}
