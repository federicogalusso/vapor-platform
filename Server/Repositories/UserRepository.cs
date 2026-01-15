using Entities;
using Interfaces.Repositories;

namespace Server.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public async Task<string> Add(User user)
    {
        return await Task.Run(() =>
        {
            try
            {
                _users.Add(user);
                return "Usuario agregado correctamente!";
            }
            catch (Exception e)
            {
                return "Error adding user: " + e.Message;
            }
        });
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await Task.Run(() =>
        {
            try
            {
                return _users.FirstOrDefault(u => u.Username == username);
            }
            catch (Exception)
            {
                return null;
            }
        });
    }

    public async Task<string> Delete(User user)
    {
        return await Task.Run(() =>
        {
            try
            {
                _users.Remove(user);
                return "User deleted successfully";
            }
            catch (Exception)
            {
                return "Error deleting user";
            }
        });
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await Task.Run(() => _users);
    }
}