namespace Mobility.Payments.Api.Tests
{
    using Moq;
    using Microsoft.Extensions.Logging;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Mobility.Payments.Controllers;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Api.Contracts.Request;
    using Mobility.Payments.Api.Contracts.Response;
    using Mobility.Payments.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using Mobility.Payments.Crosscuting.Enums;
    using Mobility.Payments.Application.Models;

    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> paymentServiceMock;
        private readonly Mock<ILogger<PaymentController>> loggerMock;
        private readonly Mock<IMapper> mapperMock;

        public PaymentControllerTests()
        {
            this.paymentServiceMock = new Mock<IPaymentService>();
            this.loggerMock = new Mock<ILogger<PaymentController>>();
            this.mapperMock = new Mock<IMapper>();

            this.Sut = new PaymentController(
                this.paymentServiceMock.Object,
                this.loggerMock.Object,
                this.mapperMock.Object);
        }

        public PaymentController Sut { get; set; }

        [Fact]
        public async Task GetPayments_OkResult()
        {
            // Arrange
            var payment = new PaymentDto { Id = Guid.NewGuid(), SenderId = "senderUsername", ReceiverId = "receiverUsername", PaymentStatus = PaymentStatus.Approved, Amount = 200 };
            this.paymentServiceMock.Setup(s => s.GetPayments()).ReturnsAsync(new List<PaymentDto> { payment }).Verifiable();
            mapperMock.Setup(mapper => mapper.Map<IEnumerable<PaymentResponse>>(It.IsAny<IEnumerable<PaymentDto>>()))
                       .Returns(new List<PaymentResponse> { new PaymentResponse { SenderId = payment.SenderId, ReceiverId = payment.ReceiverId, Amount = payment.Amount, PaymentStatus = payment.PaymentStatus } });

            // Act
            var result = await this.Sut.GetPayments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<List<PaymentResponse>>(okResult.Value);
            Assert.Single(response);
        }

        [Fact]
        public async Task GetPaymentById_OkResult()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var payment = new PaymentDto { Id = paymentId, SenderId = "senderUsername", ReceiverId = "receiverUsername", PaymentStatus = PaymentStatus.Approved, Amount = 200 };
            this.paymentServiceMock.Setup(service => service.GetPaymentById(It.IsAny<Guid>())).ReturnsAsync(payment);
            mapperMock.Setup(mapper => mapper.Map<PaymentResponse>(It.IsAny<PaymentDto>()))
                       .Returns(new PaymentResponse { SenderId = payment.SenderId, ReceiverId = payment.ReceiverId, Amount = payment.Amount, PaymentStatus = payment.PaymentStatus });

            // Act
            var result = await this.Sut.GetPaymentById(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PaymentResponse>(okResult.Value);
            Assert.Equal(payment.SenderId, response.SenderId);
            Assert.Equal(payment.ReceiverId, response.ReceiverId);
            Assert.Equal(payment.Amount, response.Amount);
            Assert.Equal(payment.PaymentStatus, response.PaymentStatus);
        }

        [Fact]
        public async Task CreatePayment_OkResult()
        {
            // Arrange
            var paymentRequest = new CreatePaymentRequest { Receiver = "receiver1", Amount = 100 };
            var transactionId = Guid.NewGuid();
            paymentServiceMock.Setup(service => service.CreateTransaction(paymentRequest.Receiver, paymentRequest.Amount))
                               .ReturnsAsync(transactionId);

            // Act
            var result = await this.Sut.CreatePayment(paymentRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(transactionId, okResult.Value);
        }

        [Fact]
        public async Task ConfirmPayment_OkResult()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            paymentServiceMock.Setup(service => service.ConfirmTransaction(paymentId))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await this.Sut.ConfirmPayment(paymentId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}