using Entities;

namespace Interfaces.Repositories
{
    public interface IGameRepository
    {
        Task Add(Game game);
        Task<Game?> GetByTitle(string title);
        Task<IEnumerable<Game>> GetAll(Func<Game, bool>? filter);
        Task<IEnumerable<Game>> GetGamesByGenre(string genre);
        Task<IEnumerable<Game>> GetGamesByPlatform(string platform);
        Task Update(string originalTitle, Game game);
        Task Delete(string title);
    }
}