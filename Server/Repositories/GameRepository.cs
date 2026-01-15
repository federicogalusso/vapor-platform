using Entities;
using Interfaces.Repositories;

namespace Server.Repositories;

public class GameRepository : IGameRepository
{
    private readonly List<Game> _games = new();

    public async Task Add(Game game)
    {
        await Task.Run(() => _games.Add(game));
    }

    public async Task<Game?> GetByTitle(string title)
    {
        return await Task.Run(() => _games.FirstOrDefault(g => g.Title == title));
    }

    public async Task<IEnumerable<Game>> GetAll(Func<Game, bool>? filter)
    {
        return await Task.Run(() => filter == null ? _games : _games.Where(filter));
    }

    public async Task<IEnumerable<Game>> GetGamesByGenre(string genre)
    {
        return await Task.Run(() => _games.Where(g => g.Type.Equals(genre, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IEnumerable<Game>> GetGamesByPlatform(string platform)
    {
        return await Task.Run(() => _games.Where(g => g.Platform.Equals(platform, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task Update(string originalTitle, Game game)
    {
        await Task.Run(() =>
        {
            var existingGame = _games.FirstOrDefault(g => g.Title == originalTitle);
            if (existingGame != null)
            {
                existingGame.Title = game.Title;
                existingGame.Type = game.Type;
                existingGame.LaunchDate = game.LaunchDate;
                existingGame.Platform = game.Platform;
                existingGame.Publisher = game.Publisher;
                existingGame.AvailableUnits = game.AvailableUnits;
                existingGame.Image = game.Image;
            }
        });
    }

    public async Task Delete(string title)
    {
        await Task.Run(() =>
        {
            var game = _games.FirstOrDefault(g => g.Title == title);
            if (game != null)
            {
                _games.Remove(game);
            }
        });
    }
}