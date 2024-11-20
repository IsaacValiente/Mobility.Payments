namespace Mobility.Payments.Api.Contracts.Request
{
    public class CreatePaymentRequest
    {
        public string Receiver { get; set; }
        public decimal Amount { get; set; }
    }
}