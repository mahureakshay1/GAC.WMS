using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> Get(CancellationToken cancellationToken)
        {
            return await _productService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<ProductDto?> Get(int id, CancellationToken cancellationToken)
        {
            return await _productService.GetByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<ProductDto> Post([FromBody] ProductDto dto, CancellationToken cancellationToken)
        {
            return await _productService.CreateAsync(dto, cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task<ProductDto> Put([FromBody] ProductDto dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id, CancellationToken cancellationToken)
        {
            return await _productService.DeleteAsync(id, cancellationToken);
        }
    }
}
