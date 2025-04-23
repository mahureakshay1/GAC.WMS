using GAC.WMS.Application.DTOs;
using GAC.WMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
        }

        [HttpPost]
        public string Post([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            return _authService.Authenticate(dto, cancellationToken);
        }
    }
}
