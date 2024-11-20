namespace Mobility.Payments.Application.Interfaces
{
    using Mobility.Payments.Application.Models;
    using System.Threading.Tasks;

    public interface IUserService
    {
        Task CreateUserAsync(UserDto userDto);
        Task<string> Login(string username, string password);
    }
}
