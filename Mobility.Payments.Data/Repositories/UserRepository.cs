namespace Mobility.Payments.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Mobility.Payments.Domain.Entities;
    using Mobility.Payments.Domain.Repositories;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger) : Repository<User>(context, logger), IUserRepository
    {
        public async Task<User> GetByUsernameAsync(string username)
        {
            this.logger.LogInformation("Getting user with username: {username}", username);
            return await this.context.User.Where(u => u.Username == username).SingleOrDefaultAsync();
        }
    }
}
