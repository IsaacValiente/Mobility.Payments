namespace Mobility.Payments.Crosscuting.Exception
{
    using System;

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
