namespace Mobility.Payments.Application.Tests
{
    using Moq;
    using Xunit;
    using Mobility.Payments.Application.Services;
    using Mobility.Payments.Application.Models;
    using Microsoft.Extensions.Logging;
    using System;

    public class UserContextProviderServiceTests
    {
        private readonly Mock<ILogger<UserContextProviderService>> _loggerMock;
        private readonly UserContextProviderService _userContextProviderService;

        public UserContextProviderServiceTests()
        {
            _loggerMock = new Mock<ILogger<UserContextProviderService>>();
            _userContextProviderService = new UserContextProviderService(_loggerMock.Object);
        }

        [Fact]
        public void SetCurrentUser_IsSuccessful_UserNotNull()
        {
            // Arrange
            var user = new UserContext { Username = "testUser" };

            // Act
            _userContextProviderService.SetCurrentUser(user);

            // Assert
            Assert.True(_userContextProviderService.HasCurrentUser);
            var currentUser = _userContextProviderService.GetCurrentUser();
            Assert.Equal("testUser", currentUser.Username);
        }

        [Fact]
        public void SetCurrentUser_ThrowArgumentNullException_UserIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _userContextProviderService.SetCurrentUser(null));
            Assert.Equal("Cannot set the current user to null. (Parameter 'user')", exception.Message);
        }

        [Fact]
        public void GetCurrentUser_ReturnsCurrentUser_WhenUserIsSet()
        {
            // Arrange
            var user = new UserContext { Username = "testUser" };
            _userContextProviderService.SetCurrentUser(user);

            // Act
            var currentUser = _userContextProviderService.GetCurrentUser();

            // Assert
            Assert.Equal("testUser", currentUser.Username);
        }

        [Fact]
        public void GetCurrentUser_ThrowInvalidOperationException_UserIsNotSet()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _userContextProviderService.GetCurrentUser());
            Assert.Equal("The current user is not set. Please set it before using it.", exception.Message);
        }

        [Fact]
        public void GetCurrentUserUsername_ReturnUsername_UserIsSet()
        {
            // Arrange
            var user = new UserContext { Username = "testUser" };
            _userContextProviderService.SetCurrentUser(user);

            // Act
            var username = _userContextProviderService.GetCurrentUserUsername();

            // Assert
            Assert.Equal("testUser", username);
        }

        [Fact]
        public void GetCurrentUserUsername_ReturnEmptyString_UserIsNotSet()
        {
            // Act
            var username = _userContextProviderService.GetCurrentUserUsername();

            // Assert
            Assert.Equal(string.Empty, username);
        }
    }
}
