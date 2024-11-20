namespace Mobility.Payments.Application.Tests
{
    using Moq;
    using Xunit;
    using System.Threading.Tasks;
    using Mobility.Payments.Application.Services;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Domain.Repositories;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Crosscuting.Exception;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Crosscuting.Enums;

    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenProviderService> _tokenProviderServiceMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenProviderServiceMock = new Mock<ITokenProviderService>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _tokenProviderServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateUser_ThrowBadRequestException_UserAlreadyExists()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserName = "existingUser",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                UserRole = UserRole.Sender
            };

            _userRepositoryMock.Setup(u => u.GetByUsernameAsync(userDto.UserName))
                               .ReturnsAsync(new User(userDto.UserName, userDto.Password, userDto.FirstName, userDto.LastName, userDto.UserRole));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _userService.CreateUserAsync(userDto));
            Assert.Equal($"User with username '{userDto.UserName}' already exists.", exception.Message);
        }

        [Theory]
        [InlineData(UserRole.Sender)]
        [InlineData(UserRole.Receiver)]
        public async Task CreateUser_IsSuccessful_UserDoesNotExist(UserRole userRole)
        {
            // Arrange
            var userDto = new UserDto
            {
                UserName = "newUser",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                UserRole = userRole
            };

            // Act
            await _userService.CreateUserAsync(userDto);

            // Assert
            _userRepositoryMock.Verify(u => u.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(u => u.SaveChangesAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Login_ThrowUnauthorizedException_InvalidCredentials()
        {
            // Arrange
            var username = "invalidUser";
            var password = "wrongPassword";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _userService.Login(username, password));
            Assert.Equal("Invalid username or password.", exception.Message);
        }

        [Theory]
        [InlineData(UserRole.Sender)]
        [InlineData(UserRole.Receiver)]
        public async Task Login_ReturnAuthToken_ValidCredentials(UserRole userRole)
        {
            // Arrange
            var username = "validUser";
            var password = "correctPassword";
            var authToken = "authToken123";
            var user = new User(username, password, "FirstName", "LastName", userRole);

            _userRepositoryMock.Setup(u => u.GetByUsernameAsync(username)).ReturnsAsync(user);
            _tokenProviderServiceMock.Setup(t => t.Create(user)).Returns(authToken);

            // Act
            var result = await _userService.Login(username, password);

            // Assert
            Assert.Equal(authToken, result);
            _tokenProviderServiceMock.Verify(t => t.Create(It.IsAny<User>()), Times.Once);
        }
    }
}