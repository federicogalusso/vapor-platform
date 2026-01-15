using Entities;

namespace Statistics.Data
{
    public class UserDataAccess
    {
        private readonly List<UserEvent> _userEvents;
        private readonly object _lock;
        private static UserDataAccess? _instance;
        private static readonly object _singletonLock = new();

        public UserDataAccess()
        {
            _userEvents = new List<UserEvent>();
            _lock = new object();
        }

        public static UserDataAccess GetInstance()
        {
            lock (_singletonLock)
            {
                _instance ??= new UserDataAccess();
            }
            return _instance;
        }

        public void Add(UserEvent userEvent)
        {
            lock (_lock)
            {
                _userEvents.Add(userEvent);
            }
        }

        public UserEvent[] GetUserEvents()
        {
            lock (_lock)
            {
                return _userEvents.ToArray();
            }
        }

        public int GetUsersCount()
        {
            lock (_lock)
            {
                return _userEvents.Count;
            }
        }
    }
}