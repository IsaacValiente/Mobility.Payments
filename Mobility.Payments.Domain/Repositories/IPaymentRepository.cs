namespace Mobility.Payments.Domain.Repositories
{
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Crosscuting.Enums;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetByIdAsync(Guid transactionId);
        Task<IEnumerable<Payment>> GetAsync(string userName, UserRole role);
    }
}
