using ClientGRPC;
using Grpc.Net.Client;

namespace AdminServer;

public class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:5270");
        
        var client = new GrpcGameService.GrpcGameServiceClient(channel);
        
        var menu = new Menu(client);

        CommandMapping.Initialize(menu);

        bool isRunning = true;

        while (isRunning)
        {
            int option = await menu.DisplayMenu();

            if (CommandMapping.Commands!.TryGetValue(option.ToString(), out var action))
            {
                if (option == 6)
                {
                    isRunning = false;
                }

                await action();
            }
        }
    }
}
