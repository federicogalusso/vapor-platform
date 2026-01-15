using Entities;

namespace Statistics.Data
{
    public class GameDataAccess
    {
        private readonly List<GameEvent> _gameEvents;
        private readonly object _lock;
        private static GameDataAccess? _instance;
        private static readonly object _singletonLock = new();

        public GameDataAccess()
        {
            _gameEvents = new List<GameEvent>();
            _lock = new object();
        }

        public static GameDataAccess GetInstance()
        {
            lock (_singletonLock)
            {
                _instance ??= new GameDataAccess();
            }
            return _instance;
        }

        public void Add(GameEvent gameEvent)
        {
            lock (_lock)
            {
                _gameEvents.Add(gameEvent);
            }
        }

        public IEnumerable<Game> GetAllGames(Func<GameEvent, bool>? filter)
        {
            lock (_lock)
            {
                var query = _gameEvents.AsEnumerable();
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return query.Select(gameEvent => new Game
                {
                    Title = gameEvent.Title,
                    Type = gameEvent.Type!,
                    Publisher = gameEvent.Publisher,
                    Platform = gameEvent.Platform!,
                    LaunchDate = gameEvent.LaunchDate!,
                    AvailableUnits = int.Parse(gameEvent.AvailableUnits!),
                    Owner = gameEvent.Owner,
                }).ToList();
            }
        }

        public List<GameEvent> GetAllGameEvents()
        {
            lock (_lock)
            {
                return [.. _gameEvents];
            }
        }
    }
}