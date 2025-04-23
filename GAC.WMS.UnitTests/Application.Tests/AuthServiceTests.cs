using GAC.WMS.Application.DTOs;
using GAC.WMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _configurationMock;
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(config => config["JwtSettings:Secret"]).Returns("Super_Secret_Key_that_is_use_to_sign_token_12345");
            _configurationMock.Setup(config => config["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(config => config["JwtSettings:Audience"]).Returns("TestAudience");

            _authService = new AuthService(_configurationMock.Object);
        }

        [TestMethod]
        public void Authenticate_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "admin"
            };
            var cancellationToken = CancellationToken.None;

            var token = _authService.Authenticate(loginDto, cancellationToken);

            Assert.IsNotNull(token);
            Assert.IsInstanceOfType(token, typeof(string));
            Assert.IsTrue(token.Length > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Authenticate_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            var loginDto = new LoginDto
            {
                Username = "invalidUser",
                Password = "invalidPassword"
            };
            var cancellationToken = CancellationToken.None;

            _authService.Authenticate(loginDto, cancellationToken);
        }
    }
}