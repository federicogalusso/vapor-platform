
using Exceptions;

namespace Entities
{
    public class User
    {
        private string _userName;
        private string _password;
        public required string Username
        {
            get => _userName;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("El username no puede estar vacio.");
                }
                _userName = value;
            }

        }
        public required string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    throw new DomainException("La password no puede estar vacia.");
                }
                _password = value;
            }

        }


        public string toString()
        {
            return $"Username: {Username}, Password: {Password}";
        }
    }
}