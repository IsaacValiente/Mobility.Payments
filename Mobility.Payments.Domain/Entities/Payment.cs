namespace Mobility.Payments.Domain.Entities
{
    using Mobility.Payments.Crosscuting.Exception;
    using Mobility.Payments.Crosscuting.Enums;
    using System;
    public class Payment : Entity
    {
        public virtual Guid Id { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string SenderId { get; set; }

        public virtual string ReceiverId { get; set; }

        public virtual PaymentStatus PaymentStatus { get; set; } = PaymentStatus.AwaitingApproval;

        public virtual User Sender { get; set; }

        public virtual User Receiver { get; set; }

        public void ConfirmTransaction(string username)
        {
            if (this.PaymentStatus != PaymentStatus.AwaitingApproval)
            {
                throw new BadRequestException("This transaction is not awaiting approval.");
            }

            if (!this.ReceiverId.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Your transaction can't be completed. The transaction is associated to another user");
            }
            this.PaymentStatus = PaymentStatus.Approved;
            Receiver.UpdateBalance(this.Amount);
        }

    }
}
