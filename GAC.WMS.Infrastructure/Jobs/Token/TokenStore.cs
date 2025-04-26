
namespace GAC.WMS.Infrastructure.Jobs.Token
{
    public class TokenStore
    {
        private string _token = null!;
        private DateTime _validTill;

        public void SetToken(string token, DateTime validTill)
        {
            _token = token;
            _validTill = validTill.AddMinutes(-1);
        }

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_token) && _validTill > DateTime.UtcNow;
        }

        public string GetToken()
        {
            return _token;
        }
    }
}
