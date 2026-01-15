using System.Net.Sockets;
using Communication;
namespace Client;
public class LoggedInMenu
{
    private readonly SocketHelper _socketHelper;
    private readonly TcpClient _tcpClient;

    public LoggedInMenu(SocketHelper socketHelper, TcpClient tcpClient)
    {
        _socketHelper = socketHelper;
        _tcpClient = tcpClient;
    }

    public async Task<int> DisplayLoggedInMenu()
    {
        Console.Write("\n ------------------------------\n" +
                      "|       MENÚ LOGUEADO          |" + "\n" +
                      " ------------------------------" + "\n" +
                      "|   1 - Publicar juego         |\n" +
                      "|   2 - Comprar juego          |\n" +
                      "|   3 - Modificar juego        |\n" +
                      "|   4 - Eliminar juego         |\n" +
                      "|   5 - Buscar juego           |\n" +
                      "|   6 - Calificar juego        |\n" +
                      "|   7 - Listar juegos          |\n" +
                      "|   8 - Filtrar por genero     |\n" +
                      "|   9 - Filtrar por plataforma |\n" +
                      "|   10 - Salir                 |" +
                      "\n" + " ------------------------------" + "\n\n");

        int option = await Client.Domain.Exceptions.GetValidatedMenuOption("Elija una opción (1-10):", 1, 10);
        return option;
    }

    public async Task PublishGame()
    {
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
        string? availableUnits = Console.ReadLine();
        Console.Write("Ingrese imagen del juego: ");
        string? imagePath = Console.ReadLine();
        string? imageName = imagePath.Split("\\").Last();

        string message = $"GAME_PUBLISH:{title}-{type}-{launchDate}-{platform}-{publisher}-{availableUnits}-{imageName}";
        await SendData(imagePath, message);
    }

    public async Task PurchaseGame()
    {
        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();
        string message = $"GAME_PURCHASE:{title}";
        await Task.Run(() => _socketHelper.SendMessage(message));
    }

    public async Task EditGame()
    {
        Console.Write("Ingrese el título del juego a modificar: ");
        string? originalTitle = Console.ReadLine();
        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();
        Console.Write("Ingrese tipo del juego: ");
        string? tipo = Console.ReadLine();
        Console.Write("Ingrese fecha de lanzamiento del juego con el formato MM/DD/YYYY: ");
        string? launchDate = Console.ReadLine();
        Console.Write("Ingrese plataforma del juego: ");
        string? platform = Console.ReadLine();
        Console.Write("Ingrese publicador del juego: ");
        string? publisher = Console.ReadLine();
        Console.Write("Ingrese unidades disponibles del juego: ");
        string? availableUnits = Console.ReadLine();
        Console.Write("Ingrese imagen del juego: ");
        string? imagePath = Console.ReadLine();
        string? imageName = imagePath.Split("\\").Last();

        string message = $"GAME_MODIFY:{originalTitle}-{title}-{tipo}-{launchDate}-{platform}-{publisher}-{availableUnits}-{imageName}";
        await SendData(imagePath, message);
    }

    public async Task DeleteGame()
    {
        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();

        string message = $"GAME_DELETE:{title}";
        await Task.Run(() => _socketHelper.SendMessage(message));
    }

    public async Task SearchGame()
    {
        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();

        string message = $"GAME_SEARCH:{title}";
        await Task.Run(() => _socketHelper.SendMessage(message));
    }

    public async Task RateGame()
    {
        Console.Write("Ingrese titulo del juego: ");
        string? title = Console.ReadLine();
        Console.Write("Ingrese una puntuacion de 1-5 del juego: ");
        string? score = Console.ReadLine();
        Console.Write("Ingrese su opinion sobre el juego: ");
        string? comment = Console.ReadLine();

        string message = $"GAME_RATE:{title}-{score}-{comment}";
        await Task.Run(() => _socketHelper.SendMessage(message));
    }

    public async Task ListAllGames()
    {
        string message = "GAME_ALL:";
        await Task.Run(() => _socketHelper.SendMessage(message));
    }

    public async Task ListGamesByGenre()
    {
        Console.Write("Ingrese el genero por el que desea filtrar: ");
        string? genere = Console.ReadLine();

        if (string.IsNullOrEmpty(genere?.Trim()))
        {
            Console.WriteLine("Error: el genero no puede ser null o vacio.");
        }
        else
        {
            string message = $"GAME_GENRE:{genere}";
            await Task.Run(() => _socketHelper.SendMessage(message));
        }
    }

    public async Task ListGamesByPlatform()
    {
        Console.Write("Ingrese la plataforma por la que desea filtrar: ");
        string? platform = Console.ReadLine();

        if (string.IsNullOrEmpty(platform?.Trim()))
        {
            Console.WriteLine("Error: la plataforma no puede ser null o vacia.");
        }
        else
        {
            string message = $"GAME_PLATFORM:{platform}";
            await Task.Run(() => _socketHelper.SendMessage(message));
        }
    }

    private async Task SendData(string path, string message)
    {
        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                FileCommunicationHandler fileCommunicationHandler = new FileCommunicationHandler(_tcpClient);
                await Task.Run(() => _socketHelper.SendMessage(message));
                await Task.Run(() => fileCommunicationHandler.SendFile(path));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: el archivo no fue encontrado");
            }
        }
        else
        {
            await Task.Run(() => _socketHelper.SendMessage(message));
        }
    }
}