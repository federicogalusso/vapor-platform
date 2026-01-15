using System.Collections.Concurrent;
using Entities;
using Interfaces.Statistics.Services;

public class StatisticsService : IStatisticsService
{
    private readonly ConcurrentBag<GameEvent> _gameEvents = new();
    private readonly ConcurrentBag<UserEvent> _userEvents = new();

    public void ProcessGameEvent(GameEvent gameEvent)
    {
        _gameEvents.Add(gameEvent);
    }

    public void ProcessUserEvent(UserEvent userEvent)
    {
        _userEvents.Add(userEvent);
    }

    public IEnumerable<Game> GetFilteredGames(Func<GameEvent, bool> filter)
    {
        return _gameEvents.Where(filter).Select(gameEvent => new Game
        {
            Title = gameEvent.Title,
            Type = "sd",
            Publisher = "sd",
            Platform = "sd",
            LaunchDate = "sd",
            AvailableUnits = 0,
            Owner="Admin"
        });
    }

    public int GetUserLoginCount()
    {
        return _userEvents.Count();
    }

    public void AddEvent(GameEvent gameEvent)
    {
        _gameEvents.Add(gameEvent);
    }

    public IEnumerable<GameEvent> GetEvents(Func<GameEvent, bool> filter)
    {
        return _gameEvents.Where(filter);
    }
}