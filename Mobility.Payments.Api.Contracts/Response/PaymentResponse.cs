namespace Mobility.Payments.Api.Contracts.Response
{
    using Mobility.Payments.Crosscuting.Enums;


    public class PaymentResponse
    {
        public virtual decimal Amount { get; set; }

        public virtual string SenderId { get; set; }

        public virtual string ReceiverId { get; set; }

        public virtual PaymentStatus PaymentStatus { get; set; }
    }
}
