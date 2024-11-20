namespace Mobility.Payments.Application.Services
{
    using AutoMapper;
    using Mobility.Payments.Application.Interfaces;
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Crosscuting.Enums;
    using Mobility.Payments.Crosscuting.Exception;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Domain.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for handling payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserContextProviderService userContextProvider;
        private readonly IMapper mapper;

        public PaymentService(IPaymentRepository paymentRepository, IUserRepository userRepository, IUserContextProviderService userContextProvider, IMapper mapper)
        {
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userContextProvider = userContextProvider ?? throw new ArgumentNullException(nameof(userContextProvider));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves all payments for the current user based on their role.
        /// </summary>
        public async Task<IEnumerable<PaymentDto>> GetPayments()
        {
            var currentUser = userContextProvider.GetCurrentUser();
            var transactions = await this.paymentRepository.GetAsync(currentUser.Username, currentUser.UserRole);
            return this.mapper.Map<IEnumerable<PaymentDto>>(transactions);
        }

        /// <summary>
        /// Retrieves a specific payment by its transaction ID, ensuring the current user has permission.
        /// </summary>
        public async Task<PaymentDto> GetPaymentById(Guid transactionId)
        {
            var currentUsername = userContextProvider.GetCurrentUserUsername();
            var transaction = await this.paymentRepository.GetByIdAsync(transactionId) ?? throw new NotFoundException($"Transaction with ID {transactionId} was not found.");
            if (!IsUserAuthorized(transaction, currentUsername))
            {
                throw new UnauthorizedException("You do not have permission to view this transaction.");
            }
            return this.mapper.Map<PaymentDto>(transaction);
        }

        /// <summary>
        /// Creates a new transaction between the sender and receiver after performing validation.
        /// </summary>
        public async Task<Guid> CreateTransaction(string receiverUsername, decimal amount)
        {
            var senderUsername = userContextProvider.GetCurrentUserUsername();
            ValidateTransaction(senderUsername, receiverUsername, amount);

            var sender = await this.userRepository.GetByUsernameAsync(senderUsername);
            var receiver = await this.userRepository.GetByUsernameAsync(receiverUsername);

            ValidateUsersForTransaction(sender, receiver, receiverUsername, amount);
            
            sender.UpdateBalance(-amount);
            var payment = new Payment
            {
                Amount = amount,
                SenderId = senderUsername,
                ReceiverId = receiverUsername,
            };

            await this.paymentRepository.AddAsync(payment);
            await this.paymentRepository.SaveChangesAsync(payment);

            return payment.Id;
        }

        /// <summary>
        /// Confirms a payment transaction after validating the receiver's identity.
        /// </summary>
        public async Task ConfirmTransaction(Guid transactionId)
        {
            var receiverUsername = userContextProvider.GetCurrentUserUsername();
            var payment = await this.paymentRepository.GetByIdAsync(transactionId) ?? throw new NotFoundException($"Transaction with ID {transactionId} not found.");
            payment.ConfirmTransaction(receiverUsername);
            await this.paymentRepository.UpdateAsync(payment);
        }

        private static bool IsUserAuthorized(Payment transaction, string currentUsername)
        {
            return transaction.SenderId.Equals(currentUsername, StringComparison.OrdinalIgnoreCase) || transaction.ReceiverId.Equals(currentUsername, StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidateTransaction(string senderUsername, string receiverUsername, decimal amount)
        {
            if (senderUsername.Equals(receiverUsername, StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Sender and receiver cannot be the same.");
            }

            if (amount <= 0)
            {
                throw new BadRequestException("Transaction amount must be greater than zero.");
            }
        }

        private static void ValidateUsersForTransaction(User sender, User receiver, string receiverUsername, decimal amount)
        {
            if (receiver == null)
            {
                throw new NotFoundException($"Receiver {receiverUsername} not found.");
            }

            if (receiver.UserRole != UserRole.Receiver)
            {
                throw new BadRequestException($"User {receiverUsername} cannot accept payments.");
            }

            if (sender.Balance < amount)
            {
                throw new BadRequestException("Sender has insufficient balance.");
            }
        }
    }
}
