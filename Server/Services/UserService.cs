using Interfaces.Repositories;
using Entities;
using Exceptions;
using Interfaces.Services;

namespace Server.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly EventPublisher _eventPublisher;


    public UserService(IUserRepository userRepository, EventPublisher eventPublisher)
    {
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<string> CreateUser(string username, string password)
    {
        if (await _userRepository.GetByUsername(username) != null)
        {
            return "El usuario ya existe";
        }

        int id = (await _userRepository.GetAll()).Count() + 1;

        var user = new Entities.User { Username = username, Password = password };
        return await _userRepository.Add(user);
    }

    public async Task<string> LoginUser(string username, string password)
    {
        if (await AuthenticateUser(username, password))
        {
            var userEvent = new UserEvent
            {
                EventName = "Login",
                UserId = username,
                CreatedAt = DateTime.Now
            };

            _eventPublisher.PublishUserEvent(userEvent);

            return "LOGIN_SUCCESS";
        }
        else
        {
            throw new LogicException("Usuario o contraseña incorrectos");
        }
    }

    public async Task<string> DeleteUser(string username)
    {
        var user = await _userRepository.GetByUsername(username);
        if (user != null)
        {
            return await _userRepository.Delete(user);
        }
        else
        {
            return "User not found";
        }
    }

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        var user = await _userRepository.GetByUsername(username);
        return user != null && user.Password == password;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _userRepository.GetByUsername(username);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }
}