using Entities;
using Exceptions;
using Interfaces.Repositories;
using Interfaces.Services;

namespace Server.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IRateRepository _rateRepository;
    private readonly EventPublisher _eventPublisher;

    public GameService(IGameRepository gameRepository, IRateRepository rateRepository, EventPublisher eventPublisher)
    {
        _gameRepository = gameRepository;
        _rateRepository = rateRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<string> PublishGame(string title, string type, string launchDate, string platform, string publisher, int availableUnits, string image, string owner)
    {
        if (await GetGameByTitle(title) != null)
        {
            return "Error: Juego duplicado";
        }

        try {
            var game = new Game
            {
                Title = title,
                Type = type,
                LaunchDate = launchDate,
                Platform = platform,
                Publisher = publisher,
                AvailableUnits = availableUnits,
                Owner = owner,
                Image = image
            };
            await _gameRepository.Add(game);

            var gameEvent = new GameEvent
            {
                EventType = "Publish",
                Publisher = publisher,
                Title = title,
                Type = type,
                LaunchDate = launchDate,
                Platform = platform,
                Image = image,
                AvailableUnits = availableUnits.ToString(),
                Owner = owner
            };

            _eventPublisher.PublishGameEvent(gameEvent);

            return "Juego publicado";
        }
        catch (DomainException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> ModifyGame(string originalTitle, string title, string type, string launchDate, string platform, string publisher, int availableUnits, string image, string owner)
    {
        if (!await IsOwner(originalTitle, owner) && !owner.Equals("Admin"))
        {
            return "No puedes editar un juego que no es tuyo.";
        }
        if (await DoesGameExist(originalTitle))
        {
            try
            {
                var game = new Game
                {
                    Title = title,
                    Type = type,
                    LaunchDate = launchDate,
                    Platform = platform,
                    Publisher = publisher,
                    AvailableUnits = availableUnits,
                    Owner = owner,
                    Image = image
                };
                await _gameRepository.Update(originalTitle, game);

                var gameEvent = new GameEvent
                {
                    EventType = "Modify",
                    Publisher = publisher,
                    Title = title,
                    Type = type,
                    LaunchDate = launchDate,
                    Platform = platform,
                    Image = image,
                    AvailableUnits = availableUnits.ToString(),
                    Owner = owner
                };

                _eventPublisher.PublishGameEvent(gameEvent);

                return "Juego editado";
            }
            catch (DomainException ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        
        return "No existe un juego con el titulo ingresado.";
    }

    public async Task<string> DeleteGame(string title, string owner)
    {
        if (!await IsOwner(title, owner) && !owner.Equals("Admin"))
        {
            return "No puedes eliminar un juego que no es tuyo.";
        }
        if (await DoesGameExist(title))
        {
            await _gameRepository.Delete(title);

            var gameEvent = new GameEvent
            {
                EventType = "Delete",
                Title = title
            };

            _eventPublisher.PublishGameEvent(gameEvent);

            return "Juego borrado";
        }
        
        return "No existe un juego con el titulo ingresado.";
    }

    public async Task<string> PurchaseGame(string title)
    {
        var game = await _gameRepository.GetByTitle(title);
        if (game != null && game.AvailableUnits >= 1)
        {
            game.AvailableUnits -= 1;
            await _gameRepository.Update(title, game);

            var gameEvent = new GameEvent
            {
                EventType = "Purchase",
                Publisher = game.Publisher,
                Title = title
            };

            _eventPublisher.PublishGameEvent(gameEvent);
            _eventPublisher.NotifyAdmins(gameEvent);

            return "Juego comprado";
        }
        else if (game == null)
        {
            throw new LogicException("No existe el juego que desea comprar");
        }
        else
        {
            throw new LogicException("No hay suficientes unidades disponibles");
        }
    }

    public async Task<Game?> GetGameByTitle(string title)
    {
        return await _gameRepository.GetByTitle(title);
    }

    public async Task<string?> GetGameByTitleString(string title)
    {
        if (await DoesGameExist(title))
        {
            var gameEntity = await _gameRepository.GetByTitle(title);
            var game = gameEntity?.ToString();
            var gameRates = await _rateRepository.GetRatesByGameTitle(title);
            foreach (var rate in gameRates!)
            {
                game += "\n" + rate.ToString();
            }
            return game;
        }
        
        return "No existe el juego con el titulo ingresado.";
    }

    public async Task<string?> GetImageFromTitle(string title)
    {
        var game = await GetGameByTitle(title);
        return game?.Image;
    }

    public async Task<IEnumerable<Game>> GetAllGames(Func<Game, bool>? filter)
    {
        return await _gameRepository.GetAll(filter);
    }

    public async Task<IEnumerable<Game>> GetGamesByGenre(string genre)
    {
        return await _gameRepository.GetGamesByGenre(genre);
    }

    public async Task<IEnumerable<Game>> GetGamesByPlatform(string platform)
    {
        return await _gameRepository.GetGamesByPlatform(platform);
    }

    public async Task<bool> DoesGameExist(string title)
    {
        var existingGame = await _gameRepository.GetByTitle(title);
        return existingGame != null;
    }
    
    private async Task<bool> IsOwner(string title, string owner)
    {
        var game = await GetGameByTitle(title);
        return game?.Owner == owner;
    }
}