using GAC.WMS.Application.DTOs;

namespace GAC.WMS.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<string> AuthenticateAsync(LoginDto loginModel, CancellationToken cancellationToken);
    }
}
