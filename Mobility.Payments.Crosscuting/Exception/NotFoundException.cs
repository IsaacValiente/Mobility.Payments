namespace Mobility.Payments.Crosscuting.Exception
{
    using System;

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
