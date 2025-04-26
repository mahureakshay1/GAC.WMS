using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GAC.WMS.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerDto>> Get(CancellationToken cancellationToken)
        {
            return await _customerService.GetAllAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<CustomerDto?> Get(int id, CancellationToken cancellationToken)
        {
            return await _customerService.GetByIdAsync(id, cancellationToken);
        }

        [HttpPost]
        public async Task<CustomerDto> Post([FromBody] CustomerDto dto, CancellationToken cancellationToken)
        {
            return await _customerService.CreateAsync(dto, cancellationToken);
        }

        [HttpPut]
        public async Task<CustomerDto> Put([FromBody] CustomerDto dto,CancellationToken cancellationToken)
        {
            return await _customerService.UpdateAsync(dto, cancellationToken);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id,CancellationToken cancellationToken)
        {
            return await _customerService.DeleteAsync(id, cancellationToken);
        }
    }
}
