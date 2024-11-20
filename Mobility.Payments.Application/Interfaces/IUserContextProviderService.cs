namespace Mobility.Payments.Application.Interfaces
{
    using Mobility.Payments.Application.Models;

    public interface IUserContextProviderService
    {
        bool HasCurrentUser { get; }

        string GetCurrentUserUsername();

        UserContext GetCurrentUser();
    }
}
