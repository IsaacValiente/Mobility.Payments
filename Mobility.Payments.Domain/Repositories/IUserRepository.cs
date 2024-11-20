namespace Mobility.Payments.Domain.Repositories
{
    using Mobility.Payments.Domain.Entities;
    using System.Threading.Tasks;

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
    }
}
