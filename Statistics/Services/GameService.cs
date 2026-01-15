using Entities;
using Interfaces.Statistics.Services;
using Statistics.Data;

namespace Statistics.Services
{
    public class GameService : IGameServiceForController
    {
        public IEnumerable<Game> GetAll(Func<GameEvent, bool> filter)
        {
            var games = GameDataAccess.GetInstance();
            return games.GetAllGames(filter);
        }
    }
}