namespace Mobility.Payments.Application.Tests
{
    using Moq;
    using Xunit;
    using Microsoft.Extensions.Options;
    using Mobility.Payments.Application.Services;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Crosscuting.Options;
    using System;
    using Mobility.Payments.Crosscuting.Enums;

    public class TokenProviderServiceTests
    {
        private readonly Mock<IOptions<JwtConfiguration>> _jwtConfigMock;
        private readonly TokenProviderService _tokenProviderService;
        private readonly JwtConfiguration _jwtConfiguration;

        public TokenProviderServiceTests()
        {
            // Setup configuration
            _jwtConfiguration = new JwtConfiguration
            {
                Secret = "this-is-a-super-secret-key-for-testing",
                Issuer = "testIssuer",
                Audience = "testAudience",
                ExpirationInMinutes = 60
            };

            // Mock IOptions
            _jwtConfigMock = new Mock<IOptions<JwtConfiguration>>();
            _jwtConfigMock.Setup(opt => opt.Value).Returns(_jwtConfiguration);

            // Instantiate TokenProviderService with mock configuration
            _tokenProviderService = new TokenProviderService(_jwtConfigMock.Object);
        }

        [Fact]
        public void Create_GeneratesToken_UserIsNotNull()
        {
            // Arrange
            var user = new User
            {
                Username = "testUser",
                UserRole = UserRole.Sender
            };

            // Act
            var token = _tokenProviderService.Create(user);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public void Create_ThrowsArgumentNullException_UserIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _tokenProviderService.Create(null));
            Assert.Equal("User cannot be null. (Parameter 'user')", exception.Message);
        }
    }
}
