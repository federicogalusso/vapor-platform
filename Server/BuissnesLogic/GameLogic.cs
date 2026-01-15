using System.Text;
using Server.Services;
using Entities;
using Exceptions;
using Interfaces.Services;

namespace Server.BuissnesLogic;

public class GameLogic
{
    private readonly IGameService service;
    private readonly object _lock = new object();

    public GameLogic(GameService service)
    {
        this.service = service;
    }


    public string PublishGame(string[] param)
    {
        var title = param[0];
        var type = param[1];
        var launchDate = param[2];
        var platform = param[3];
        var publisher = param[4];
        if (!int.TryParse(param[5], out int availableUnits))
        {
            return "Error: La cantidad de unidades debe ser un numero valido.";
        }
        var image = param[6];
        var imagePath = getImagePath(image);
        var owner = param[7];

        string result;
        lock (_lock)
        {
            try
            {
                result = service.PublishGame(title, type, launchDate, platform,
                    publisher, availableUnits, imagePath, owner).Result;
            }
            catch (DomainException ex)
            {
                return $"Error: {ex.Message}";
            }
            catch (LogicException)
            {
                return "Error: Juego duplicado";
            }
        }
        return result;
    }

    public async Task<Game?> GetGameByTitle(string title)
    {
        Task<Game?> task;
        lock (_lock)
        {
            task = service.GetGameByTitle(title);
        }
        return await task;
    }

    public string? GetGameByTitleString(string[] param)
    {
        string title = param[0];
        string result;
        lock (_lock)
        {
            try
            {
                result = service.GetGameByTitleString(title).Result;
            }
            catch (LogicException ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        return result;
    }

    public async Task<string?> GetGameImage(string title)
    {
        try
        {
            return await service.GetImageFromTitle(title);
        }
        catch (LogicException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public string GetAllGames()
    {
        Func<Game, bool>? filter = null;
        List<Game> games;
        lock (_lock)
        {
            games = (List<Game>)service.GetAllGames(filter).Result;
        }
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("\n");
        foreach (var game in games)
        {
            stringBuilder.AppendLine(game.ToString());
        }

        return stringBuilder.ToString();
    }

    public string GetGamesByGenre(string[] param)
    {
        string type = param[0];
        List<Game> games;
        lock (_lock)
        {
            try
            {
                games = (List<Game>)service.GetGamesByGenre(type).Result;
            }
            catch (LogicException ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        var stringBuilder = new StringBuilder();
        foreach (var game in games)
        {
            stringBuilder.AppendLine(game.ToString());
        }

        return stringBuilder.ToString();
    }

    public string GetGamesByPlatform(string[] param)
    {
        string platform = param[0];
        List<Game> games;
        lock (_lock)
        {
            try
            {
                games = (List<Game>)service.GetGamesByPlatform(platform).Result;
            }
            catch (LogicException ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        var stringBuilder = new StringBuilder();
        foreach (var game in games)
        {
            stringBuilder.AppendLine(game.ToString());
        }

        return stringBuilder.ToString();
    }

    public async Task<string> ModifyGame(string[] param)
    {
        string originalTitle = param[0];
        string title;
        string type;
        string launchDate;
        string platform;
        string publisher;
        int availableUnits;
        string image;
        string imagePath;
        string owner = param[8];
        lock (_lock)
        {
            try
            {
                title = param[1];
                type = param[2];
                launchDate = param[3];
                platform = param[4];
                publisher = param[5];
                if (!int.TryParse(param[6], out availableUnits))
                {
                    return "Error: La cantidad de unidades debe ser un numero valido";
                }
                image = param[7];
                imagePath = getImagePath(image);


            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        return await service.ModifyGame(originalTitle, title, type, launchDate,
            platform, publisher, availableUnits, imagePath, owner);
    }

    public string DeleteGame(string[] param)
    {
        var title = param[0];
        var owner = param[1];
        string result;
        lock (_lock)
        {
            try
            {
                result = service.DeleteGame(title, owner).Result;
            }
            catch (LogicException ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        return result;
    }

    public string PurchaseGame(string[] param)
    {
        string title = param[0];
        string result;
        lock (_lock)
        {
            try
            {
                result = service.PurchaseGame(title).Result;
            }
            catch (LogicException ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        return result;
    }

    private string getImagePath(string image)
    {
        return !string.IsNullOrEmpty(image)
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageFiles", image)
            : "";
    }
}