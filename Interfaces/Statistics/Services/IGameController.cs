using Entities;

namespace Interfaces.Statistics.Services
{
    public interface IGameServiceForController
    {
        IEnumerable<Game> GetAll(Func<GameEvent, bool> filter);
    }
}