namespace Mobility.Payments.Application.Models
{
    using Mobility.Payments.Crosscuting.Enums;

    public class UserContext
    {
        public string Username { get; set; }
        public UserRole UserRole { get; set; }
    }
}
