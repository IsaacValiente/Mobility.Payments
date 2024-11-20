namespace Mobility.Payments.Application.Tests
{
    using Moq;
    using Xunit;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Mobility.Payments.Application.Services;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Domain.Repositories;
    using Mobility.Payments.Domain.Entities;
    using AutoMapper;
    using Mobility.Payments.Crosscuting.Exception;
    using Mobility.Payments.Crosscuting.Enums;

    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> paymentRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IUserContextProviderService> userContextProviderMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly PaymentService paymentService;

        public PaymentServiceTests()
        {
            paymentRepositoryMock = new Mock<IPaymentRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            userContextProviderMock = new Mock<IUserContextProviderService>();
            mapperMock = new Mock<IMapper>();

            paymentService = new PaymentService(
                paymentRepositoryMock.Object,
                userRepositoryMock.Object,
                userContextProviderMock.Object,
                mapperMock.Object
            );
        }

        [Theory]
        [InlineData(UserRole.Sender)]
        [InlineData(UserRole.Receiver)]
        public async Task GetPayments_ValidUser_Success(UserRole userRole)
        {
            // Arrange
            var currentUser = new UserContext { Username = "user1", UserRole = userRole };
            var payments = new List<Payment>
            {
                new Payment { Id = Guid.NewGuid(), SenderId = "user1", ReceiverId = "user2", Amount = 100 },
                new Payment { Id = Guid.NewGuid(), SenderId = "user2", ReceiverId = "user1", Amount = 100 }
            };
            userContextProviderMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            paymentRepositoryMock.Setup(p => p.GetAsync(currentUser.Username, currentUser.UserRole))
                                  .ReturnsAsync(payments);
            mapperMock.Setup(m => m.Map<IEnumerable<PaymentDto>>(payments))
                       .Returns(new List<PaymentDto> {
                new PaymentDto { Id = Guid.NewGuid(), SenderId = "user1", ReceiverId = "user2", Amount = 100 },
                new PaymentDto { Id = Guid.NewGuid(), SenderId = "user2", ReceiverId = "user1", Amount = 100 }
            });

            // Act
            var result = await paymentService.GetPayments();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            paymentRepositoryMock.Verify(p => p.GetAsync(It.IsAny<string>(), It.IsAny<UserRole>()), Times.Once);
        }

        [Fact]
        public async Task GetPaymentById_ThrowNotFoundException()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns("user1");

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => paymentService.GetPaymentById(transactionId));
        }

        [Fact]
        public async Task GetPaymentById_ShouldThrowUnauthorizedException_WhenUserNotAuthorized()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var payment = new Payment { Id = transactionId, SenderId = "user2", ReceiverId = "user3" };
            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns("user1");
            paymentRepositoryMock.Setup(p => p.GetByIdAsync(transactionId)).ReturnsAsync(payment);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => paymentService.GetPaymentById(transactionId));
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowBadRequestException_WhenSenderAndReceiverAreTheSame()
        {
            // Arrange
            var receiverUsername = "user1";
            var amount = 100m;
            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns("user1");

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task CreateTransaction_ShouldCreateTransaction_WhenValid()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user2";
            var amount = 100m;

            var sender = new User { Username = senderUsername, Balance = 200m };
            var receiver = new User { Username = receiverUsername, UserRole = UserRole.Receiver };
            var payment = new Payment { Id = new Guid(), SenderId = senderUsername, ReceiverId = receiverUsername, Amount = amount };

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(senderUsername)).ReturnsAsync(sender);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(receiverUsername)).ReturnsAsync(receiver);

            // Act
            var result = await paymentService.CreateTransaction(receiverUsername, amount);

            // Assert
            paymentRepositoryMock.Verify(p => p.AddAsync(It.IsAny<Payment>()), Times.Once);
            paymentRepositoryMock.Verify(p => p.SaveChangesAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowBadRequest_WhenNegativeAmmount()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user2";
            var amount = -100m;

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowBadRequest_WhenSenderIsReceiver()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user1";
            var amount = 100m;

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task CreateTransaction_ThrowNotFoundException_ReceiverNotFound()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user2";
            var amount = 100m;

            var sender = new User { Username = senderUsername, Balance = 200m };
            var receiver = new User { Username = receiverUsername, UserRole = UserRole.Receiver };
            var payment = new Payment { Id = new Guid(), SenderId = senderUsername, ReceiverId = receiverUsername, Amount = amount };

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(senderUsername)).ReturnsAsync(sender);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task CreateTransaction_ThrowBadRequestException_ReceiverWithSenderRole()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user2";
            var amount = 100m;

            var sender = new User { Username = senderUsername, Balance = 200m };
            var receiver = new User { Username = receiverUsername, UserRole = UserRole.Sender };
            var payment = new Payment { Id = new Guid(), SenderId = senderUsername, ReceiverId = receiverUsername, Amount = amount };

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(senderUsername)).ReturnsAsync(sender);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(receiverUsername)).ReturnsAsync(receiver);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task CreateTransaction_ThrowBadRequestException_InsufficientBalance()
        {
            // Arrange
            var senderUsername = "user1";
            var receiverUsername = "user2";
            var amount = 200m;

            var sender = new User { Username = senderUsername, Balance = 100m };
            var receiver = new User { Username = receiverUsername, UserRole = UserRole.Receiver };
            var payment = new Payment { Id = new Guid(), SenderId = senderUsername, ReceiverId = receiverUsername, Amount = amount };

            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns(senderUsername);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(senderUsername)).ReturnsAsync(sender);
            userRepositoryMock.Setup(r => r.GetByUsernameAsync(receiverUsername)).ReturnsAsync(receiver);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => paymentService.CreateTransaction(receiverUsername, amount));
        }

        [Fact]
        public async Task ConfirmTransaction_ShouldThrowNotFoundException_WhenPaymentNotFound()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns("user1");

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => paymentService.ConfirmTransaction(transactionId));
        }

        [Fact]
        public async Task ConfirmTransaction_ShouldUpdatePayment_WhenValid()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var payment = new Payment { Id = transactionId, SenderId = "user1", ReceiverId = "user2", Amount = 20, Receiver = new User { Username = "user2", Balance = 100} };
            userContextProviderMock.Setup(u => u.GetCurrentUserUsername()).Returns("user2");
            paymentRepositoryMock.Setup(p => p.GetByIdAsync(transactionId)).ReturnsAsync(payment);
            paymentRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);

            // Act
            await paymentService.ConfirmTransaction(transactionId);

            // Assert
            paymentRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<Payment>()), Times.Once);
        }
    }
}
