using Entities;
using Microsoft.AspNetCore.Mvc;
using Statistics.Data;

namespace Statistics.Controllers;

[ApiController]
[Route("games")]
public class GamesController(ILogger<GamesController> logger, GameDataAccess gameDataAccess) : ControllerBase
{
    private readonly ILogger<GamesController> _logger = logger;

    [HttpGet]
    public IEnumerable<Game> GetGames([FromQuery] string? platform, [FromQuery] string? publisher, [FromQuery] string? type)
    {
        Func<GameEvent, bool> filter = game => (
            (string.IsNullOrEmpty(platform) || game.Platform == platform) &&
            (string.IsNullOrEmpty(publisher) || game.Publisher == publisher) &&
            (string.IsNullOrEmpty(type) || game.Type == type) &&
            game.EventType == "Publish"
        );

        return gameDataAccess.GetAllGames(filter);
    }
}