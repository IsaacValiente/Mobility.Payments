namespace Mobility.Payments.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Api.Contracts.Request;
    using Mobility.Payments.Api.Contracts.Response;
    using Mobility.Payments.Application.Interfaces;

    /// <summary>
    /// Controller for managing payments.
    /// </summary>
    [ApiController]
    [Route("payment")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="paymentService">The service responsible for user operations.</param>
        /// <param name="logger">The logger for recording application events and messages.</param>
        /// <param name="mapper">The mapper for converting between models and DTOs.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the dependencies are null.</exception>
        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger,
            IMapper mapper)
        {
            this.paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Gets all payments for the current user.
        /// </summary>
        /// <returns>A list of payments.</returns>
        /// <response code="200">Returns the list of payments.</response>
        /// <response code="401">Unauthorized - User is not authenticated.</response>
        [HttpGet]
        [Authorize(Roles = "Sender,Receiver")]
        public async Task<IActionResult> GetPayments()
        {
            this.logger.LogInformation("Getting transactions for currentUser");
            var payments = await this.paymentService.GetPayments();
            var result = this.mapper.Map<IEnumerable<PaymentResponse>>(payments);
            return this.Ok(result);
        }

        /// <summary>
        /// Gets a payment by its ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment.</param>
        /// <returns>The payment details.</returns>
        /// <response code="200">Returns the payment details.</response>
        /// <response code="401">Unauthorized - User is not authenticated.</response>
        /// <response code="404">Payment not found.</response>
        [HttpGet("{paymentId:guid}")]
        [Authorize(Roles = "Sender,Receiver")]
        public async Task<IActionResult> GetPaymentById([FromRoute] Guid paymentId)
        {
            this.logger.LogInformation("Getting transaction with id: {paymentId}", paymentId);
            var payments = await this.paymentService.GetPaymentById(paymentId);
            var result = this.mapper.Map<PaymentResponse>(payments);
            return this.Ok(result);
        }

        /// <summary>
        /// Creates a new payment transaction.
        /// </summary>
        /// <param name="paymentRequest">The payment request details.</param>
        /// <returns>The ID of the created transaction.</returns>
        /// <response code="200">Payment created successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User is not authenticated.</response>
        [HttpPost]
        [Authorize(Roles = "Sender")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest paymentRequest)
        {
            this.logger.LogInformation("Creating a payment");
            var transactionId = await this.paymentService.CreateTransaction(paymentRequest.Receiver, paymentRequest.Amount);
            return this.Ok(transactionId);
        }

        /// <summary>
        /// Confirms a payment by its ID.
        /// </summary>
        /// <param name="paymentId">The ID of the payment to confirm.</param>
        /// <returns>Confirmation status.</returns>
        /// <response code="200">Payment confirmed successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized - User is not authenticated.</response>
        /// <response code="404">Payment not found.</response>
        [HttpPatch("{paymentId:guid}/confirm")]
        [Authorize(Roles = "Receiver")]
        public async Task<IActionResult> ConfirmPayment([FromRoute] Guid paymentId)
        {
            this.logger.LogInformation("Confirming payment with id {paymentId}", paymentId);
            await this.paymentService.ConfirmTransaction(paymentId);

            return this.Ok();
        }
    }
}
