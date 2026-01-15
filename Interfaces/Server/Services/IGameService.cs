using Entities;

namespace Interfaces.Services
{
    public interface IGameService 
    {
        Task<string> PublishGame(string title, string type, string launchDate, string platform, string publisher, int availableUnits, string image, string owner);
        Task<string> ModifyGame(string originalTitle, string title, string type, string launchDate, string platform, string publisher, int availableUnits, string image, string owner);
        Task<string> DeleteGame(string title, string owner);
        Task<string> PurchaseGame(string title);
        Task<Game?> GetGameByTitle(string title);
        Task<string?> GetGameByTitleString(string title);
        Task<string?> GetImageFromTitle(string title);
        Task<IEnumerable<Game>> GetAllGames(Func<Game, bool>? filter);
        Task<IEnumerable<Game>> GetGamesByGenre(string genre);
        Task<IEnumerable<Game>> GetGamesByPlatform(string platform);
        Task<bool> DoesGameExist(string title);

    }
}