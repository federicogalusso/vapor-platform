using System;
using System.Threading.Tasks;
using Exceptions;
using Interfaces.Services;
using Server.Services;

namespace Server.BuissnesLogic
{
    public class UserLogic
    {
        private readonly IUserService _service;
        private readonly object _lock = new object();

        public UserLogic(UserService service)
        {
            _service = service;
        }

        public string CreateUser(string[] param)
        {
            string username = param[0].Trim();
            string password = param[1].Trim();
            string result;
            lock (_lock)
            {
                try
                {
                    result = _service.CreateUser(username, password).Result;
                }
                catch (DomainException ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
            return result;
        }

        public string LoginUser(string[] param)
        {
            string username = param[0].Trim();
            string password = param[1].Trim();

            string result;
            lock (_lock)
            {
                try
                {
                    result = _service.LoginUser(username, password).Result;
                }
                catch (LogicException ex)
                {
                    return $"Error: {ex.Message}";
                }
            }

            return result;
        }
    }
}