using Entities;

namespace Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<string> Add(User user);
        Task<User?> GetByUsername(string username);
        Task<IEnumerable<User>> GetAll();
        Task<string> Delete(User user);
    }
}