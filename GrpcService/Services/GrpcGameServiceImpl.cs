using Entities;
using Grpc.Core;
using Server.BuissnesLogic;

namespace GrpcService.Services;

public class GrpcGameServiceImpl : GrpcGameService.GrpcGameServiceBase
{
    private readonly GameLogic _gameLogic;

    public GrpcGameServiceImpl(GameLogic gameLogic)
    {
        _gameLogic = gameLogic;
    }

    public override async Task<GameResponse> PublishGame(PublishGameRequest request, ServerCallContext context)
    {
        var result = _gameLogic.PublishGame([
            request.Title, request.Type, request.LaunchDate, request.Platform,
            request.Publisher, request.AvailableUnits.ToString(), request.Image, request.Owner
        ]);
        
        return new GameResponse { Message = result };
    }

    public override async Task<GameResponse> ModifyGame(ModifyGameRequest request, ServerCallContext context)
    {
        var result = await _gameLogic.ModifyGame([
            request.OriginalTitle, request.Title, request.Type, request.LaunchDate, request.Platform,
            request.Publisher, request.AvailableUnits.ToString(), request.Image, "Admin"
        ]);
        
        return new GameResponse { Message = result };
    }

    public override async Task<GameResponse> DeleteGame(GameRequest request, ServerCallContext context)
    {
        var result = _gameLogic.DeleteGame([
            request.Title, "Admin"
        ]);
        
        return new GameResponse { Message = result };
    }

    public override async Task<GameResponse> GetGameByTitle(GameRequest request, ServerCallContext context)
    {
        var result = _gameLogic.GetGameByTitleString([request.Title]);
        return new GameResponse { Message = result };
    }
    
    public override async Task<GameResponse> PurchaseGame(GameRequest request, ServerCallContext context)
    {
        var result = _gameLogic.PurchaseGame(new[] { request.Title });

        return new GameResponse { Message = result };
    }
    
    public override async Task<GameListResponse> GetAllGames(Empty request, ServerCallContext context)
    {
        var gamesString = _gameLogic.GetAllGames();

        var gamesList = gamesString.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        var gameResponses = gamesList.Select(title => new GameResponse { Message = title }).ToList();

        return new GameListResponse { Games = { gameResponses } };

    }
}
