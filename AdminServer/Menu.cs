using ClientGRPC;
namespace AdminServer;

public class Menu
{
    private readonly GrpcGameService.GrpcGameServiceClient _client;

    public Menu(GrpcGameService.GrpcGameServiceClient client)
    {
        _client = client;
    }

    public async Task<int> DisplayMenu()
    {
        Console.Write("\n ------------------------------\n" +
                      "|       MENÚ LOGUEADO          |" + "\n" +
                      " ------------------------------" + "\n" +
                      "|   1 - Publicar juego         |\n" +
                      "|   2 - Modificar juego        |\n" +
                      "|   3 - Eliminar juego         |\n" +
                      "|   4 - Consultar calificacion |\n" +
                      "|   5 - Proximas compras       |\n" +
                      "|   6 - Salir                  |\n" + 
                      " ------------------------------" + "\n\n");

        int option = await ValidateOption("Elija una opción (1-6):", 1, 6);
        return option;
    }

    public async Task PublishGame()
    {
        Console.WriteLine("\n--- Agregar Juego ---");

        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();
        Console.Write("Ingrese tipo del juego: ");
        string? type = Console.ReadLine();
        Console.Write("Ingrese fecha de lanzamiento del juego: ");
        string? launchDate = Console.ReadLine();
        Console.Write("Ingrese plataforma del juego: ");
        string? platform = Console.ReadLine();
        Console.Write("Ingrese publicador del juego: ");
        string? publisher = Console.ReadLine();
        Console.Write("Ingrese unidades disponibles del juego: ");
        if (!int.TryParse(Console.ReadLine(), out int availableUnits))
        {
            Console.WriteLine("Error: Las unidades deben ser un número.");
            return;
        }
        Console.Write("Ingrese dueño del juego: ");
        string? owner = Console.ReadLine();

        try
        {
            var response = await _client.PublishGameAsync(new PublishGameRequest
            {
                Title = title,
                Type = type,
                LaunchDate = launchDate,
                Platform = platform,
                Publisher = publisher,
                AvailableUnits = availableUnits,
                Owner = owner
            });

            Console.WriteLine($"Respuesta del servidor: {response.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar el juego: {ex.Message}");
        }
    }

    public async Task ModifyGame()
    {
        Console.WriteLine("\n--- Modificar Juego ---");

        Console.WriteLine("Ingrese el titulo del juego a modificar: ");
        string? originalTitle = Console.ReadLine();

        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();
        Console.Write("Ingrese tipo del juego: ");
        string? type = Console.ReadLine();
        Console.Write("Ingrese fecha de lanzamiento del juego: ");
        string? launchDate = Console.ReadLine();
        Console.Write("Ingrese plataforma del juego: ");
        string? platform = Console.ReadLine();
        Console.Write("Ingrese publicador del juego: ");
        string? publisher = Console.ReadLine();
        Console.Write("Ingrese unidades disponibles del juego: ");
        if (!int.TryParse(Console.ReadLine(), out int availableUnits))
        {
            Console.WriteLine("Error: Las unidades deben ser un número.");
            return;
        }

        try
        {
            var response = await _client.ModifyGameAsync(new ModifyGameRequest
            {
                OriginalTitle = originalTitle,
                Title = title,
                Type = type,
                LaunchDate = launchDate,
                Platform = platform,
                Publisher = publisher,
                AvailableUnits = availableUnits
            });

            Console.WriteLine($"Respuesta del servidor: {response.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al modificar el juego: {ex.Message}");
        }
    }

    public async Task DeleteGame()
    {
        Console.WriteLine("\n--- Eliminar Juego ---");
        Console.Write("Título: ");
        var title = Console.ReadLine();

        try
        {
            var response = await _client.DeleteGameAsync(new GameRequest { Title = title });
            Console.WriteLine($"Respuesta del servidor: {response.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar el juego: {ex.Message}");
        }
    }

    public async Task GetGameRating()
    {
        Console.WriteLine("\n--- Consultar Calificaciones ---");

        Console.Write("Título del Juego: ");
        var title = Console.ReadLine();

        try
        {
            var response = await _client.GetGameByTitleAsync(new GameRequest { Title = title });
            Console.WriteLine($"Respuesta del servidor: {response.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al consultar las calificaciones: {ex.Message}");
        }
    }

    public async Task GetNextPurchases()
    {
        Console.WriteLine("\n--- Proximas Compras ---");
        var nPurchases = GetValidNumber("Ingrese cuántas compras desea observar: ", "Ingrese un numero mayor a cero: ");
        var consumer = new PurchaseConsumer();
        consumer.StartListening(nPurchases);    
    }

    public async Task Exit()
    {
        Console.WriteLine("\nCerrando el sistema");
        await Task.CompletedTask;
        Environment.Exit(0);
    }


    private async Task<int> ValidateOption(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (Int32.TryParse(input, out result) && result >= min && result <= max)
            {
                break;
            }
            else
            {
                Console.WriteLine($"Input invalido. Porfavor ingrese un numero entre {min} y {max}.");
            }

            await Task.Yield();
        }
        return result;
    }
    
    private int GetValidNumber(string initialPrompt, string retryPrompt)
    {
        int number;
        string? input;
        bool firstAttempt = true;
        do
        {
            Console.Write(firstAttempt ? initialPrompt : retryPrompt);
            input = Console.ReadLine();
            firstAttempt = false;
        } while (!int.TryParse(input, out number) || number <= 0);

        return number;
    }
}
