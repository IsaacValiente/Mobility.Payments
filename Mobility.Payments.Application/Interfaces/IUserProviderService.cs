namespace Mobility.Payments.Application.Interfaces
{
    using Mobility.Payments.Application.Models;

    public interface IUserProviderService
    {
        void SetCurrentUser(UserContext user);
    }
}
