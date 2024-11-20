namespace Mobility.Payments.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Crosscuting.Enums;
    using Mobility.Payments.Domain.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository for managing Payment entities in the database.
    /// Provides methods for accessing and manipulating payment data.
    /// </summary>
    public class PaymentRepository(ApplicationDbContext context, ILogger<PaymentRepository> logger) : Repository<Payment>(context, logger), IPaymentRepository
    {
        /// <summary>
        /// Retrieves payments for a specific user based on their role.
        /// </summary>
        /// <param name="userName">The username of the user for which payments are to be retrieved.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>A collection of payments associated with the user.</returns>
        public async Task<IEnumerable<Payment>> GetAsync(string userName, UserRole role)
        {
            this.logger.LogInformation("Retrieving payments for user: {UserName} with role: {Role}", userName, role);

            var query = role switch
            {
                UserRole.Sender => this.context.Payment.Where(p => p.SenderId == userName),
                _ => this.context.Payment.Where(p => p.ReceiverId == userName)
            };

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific payment by its transaction ID.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the payment transaction.</param>
        /// <returns>The payment with the specified transaction ID, or null if not found.</returns>
        public async Task<Payment> GetByIdAsync(Guid transactionId)
        {
            this.logger.LogInformation("Retrieving payment with ID: {TransactionId}", transactionId);
            return await this.context.Payment.Where(u => u.Id == transactionId).SingleOrDefaultAsync();
        }
    }
}
