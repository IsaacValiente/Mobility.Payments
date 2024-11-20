namespace Mobility.Payments.Application.Models
{
    using Mobility.Payments.Crosscuting.Enums;
    using System;

    public class PaymentDto
    {
        public virtual Guid Id { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual string SenderId { get; set; }

        public virtual string ReceiverId { get; set; }

        public virtual PaymentStatus PaymentStatus { get; set; }
    }
}
