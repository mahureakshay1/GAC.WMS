using GAC.WMS.Application.DTOs;

namespace GAC.WMS.Application.Interfaces
{
    public interface IAuthService
    {
        public string Authenticate(LoginDto loginModel, CancellationToken cancellationToken);
    }
}
