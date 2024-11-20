namespace Mobility.Payments.Application.Models
{
    using Mobility.Payments.Crosscuting.Enums;

    public class UserDto
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public UserRole UserRole { get; set; }
    }
}
