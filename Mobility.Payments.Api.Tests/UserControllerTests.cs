namespace Mobility.Payments.Api.Tests
{
    using Moq;
    using Xunit;
    using Mobility.Payments.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Mobility.Payments.Application.Interfaces;
    using Microsoft.Extensions.Logging;
    using AutoMapper;
    using Mobility.Payments.Api.Contracts.Request;
    using Mobility.Payments.Application.Models;
    using System.Threading.Tasks;
    using Mobility.Payments.Crosscuting.Enums;

    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new UserController(
                _userServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnOk_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var userRequest = new CreateUserRequest
            {
                UserName = "testUser",
                Password = "testPassword",
                FirstName = "Test",
                LastName = "User",
                UserRole = UserRole.Sender,
            };

            var userDto = new UserDto
            {
                UserName = userRequest.UserName,
                Password = userRequest.Password,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                UserRole = userRequest.UserRole
            };

            _mapperMock.Setup(m => m.Map<UserDto>(userRequest)).Returns(userDto);
            _userServiceMock.Setup(s => s.CreateUserAsync(userDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(userRequest);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _userServiceMock.Verify(s => s.CreateUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var userLoginRequest = new UserLoginRequest
            {
                UserName = "testUser",
                Password = "testPassword"
            };

            var expectedAuthToken = "someToken";
            _userServiceMock.Setup(s => s.Login(userLoginRequest.UserName, userLoginRequest.Password))
                            .ReturnsAsync(expectedAuthToken);

            // Act
            var result = await _controller.Login(userLoginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authToken = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedAuthToken, authToken);
        }
    }
}