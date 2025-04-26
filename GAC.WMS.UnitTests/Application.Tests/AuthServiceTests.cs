using GAC.WMS.Application.Common.IntegrationOptions;
using GAC.WMS.Application.DTOs;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _configurationMock = null!;
        private IAuthService _authService = null!;
        private Mock<ILogger<IAuthService>> _loggerMock = null!;
        private Mock<IOptions<JwtIntegrationOptions>> _jwtOptionsMock = null!;
        private Mock<IOptions<ApplicationIntegrationOptions>> _applicationOptionsMock = null!;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(config => config["JwtSettings:Secret"]).Returns("Super_Secret_Key_that_is_use_to_sign_token_12345");
            _configurationMock.Setup(config => config["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(config => config["JwtSettings:Audience"]).Returns("TestAudience");
            _loggerMock = new Mock<ILogger<IAuthService>>();




            _applicationOptionsMock = new Mock<IOptions<ApplicationIntegrationOptions>>();
            _applicationOptionsMock.Setup(o => o.Value).Returns(new ApplicationIntegrationOptions
            {
                Username = "admin",
                Password = "admin",
                Url = "http://localhost:5000",
                Port = "5000",
            });

            _jwtOptionsMock = new Mock<IOptions<JwtIntegrationOptions>>();
            _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtIntegrationOptions
            {
                Audience = "TestAudience",
                Issuer = "TestIssuer",
                ExpirationInMinutes = 60,
                RequireExpirationTime = true,
                Secret = "Super_Secret_Key_that_is_use_to_sign_token_12345",
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true
            });

            _authService = new AuthService(_jwtOptionsMock.Object, _applicationOptionsMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Authenticate_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDto
            {
                Username = "admin",
                Password = "admin"
            };
            var cancellationToken = CancellationToken.None;

            var token = await _authService.AuthenticateAsync(loginDto, cancellationToken);

            Assert.IsNotNull(token);
            Assert.IsInstanceOfType(token, typeof(string));
            Assert.IsTrue(token.Length > 0);
        }

        [TestMethod]
        public async Task Authenticate_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            var loginDto = new LoginDto
            {
                Username = "invalidUser",
                Password = "invalidPassword"
            };
            var cancellationToken = CancellationToken.None;

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => _authService.AuthenticateAsync(loginDto, cancellationToken));
        }
    }
}