namespace Mobility.Payments.Api.Contracts.Request
{
    using Mobility.Payments.Crosscuting.Enums;

    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole UserRole { get; set; }
    }
}