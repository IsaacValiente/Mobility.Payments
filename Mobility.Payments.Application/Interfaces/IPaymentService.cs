namespace Mobility.Payments.Application.Interfaces
{
    using Mobility.Payments.Application.Models;
    using Mobility.Payments.Crosscuting.Enums;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetPayments();
        Task<PaymentDto> GetPaymentById(Guid transactionId);
        Task<Guid> CreateTransaction(string receiverUsername, decimal amount);
        Task ConfirmTransaction(Guid transactionId);
    }
}
