using GrpcService.Services;
using Interfaces.Repositories;
using Server.BuissnesLogic;
using Server.Repositories;
using Server.Services;

namespace GrpcService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5270); // Configurar puerto 5270 para HTTP
        });

        // Add services to the container.
        builder.Services.AddGrpc();

        builder.Services.AddSingleton<EventPublisher>();
        
        builder.Services.AddSingleton<IRateRepository, RateRepository>();
        builder.Services.AddSingleton<IGameRepository, GameRepository>();
        builder.Services.AddSingleton<IUserRepository, UserRepository>();

        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<GameService>();
        builder.Services.AddSingleton<RateService>();

        builder.Services.AddSingleton<UserLogic>();
        builder.Services.AddSingleton<GameLogic>();
        builder.Services.AddSingleton<RateLogic>();

        var app = builder.Build();

        app.MapGrpcService<GrpcGameServiceImpl>();

        // Configure the HTTP request pipeline.
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        _ = Task.Run(async () => await Server.Program.InitTcpServer(app.Services));

        
        app.Run();
    }
}