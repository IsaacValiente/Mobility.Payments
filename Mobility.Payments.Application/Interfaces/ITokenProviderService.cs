namespace Mobility.Payments.Application.Interfaces
{
    using Mobility.Payments.Domain.Entities;
    public interface ITokenProviderService
    {
        string Create(User user);
    }
}
