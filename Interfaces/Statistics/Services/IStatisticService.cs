using Entities;

namespace Interfaces.Statistics.Services
{
    public interface IStatisticsService
    {
        void ProcessGameEvent(GameEvent gameEvent);
        void ProcessUserEvent(UserEvent userEvent);
        IEnumerable<Game> GetFilteredGames(Func<GameEvent, bool> filter);
        int GetUserLoginCount();
        void AddEvent(GameEvent gameEvent);
        IEnumerable<GameEvent> GetEvents(Func<GameEvent, bool> filter);
    }
}