using Entities;

namespace Interfaces.Services
{
    public interface IUserService
    {
        Task<string> CreateUser(string username, string password);
        Task<string> LoginUser(string username, string password);
        Task<string> DeleteUser(string username);
        Task<bool> AuthenticateUser(string username, string password);
        Task<User?> GetUserByUsername(string username);
        Task<IEnumerable<User>> GetAllUsers();
    }
}