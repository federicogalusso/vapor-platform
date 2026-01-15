using Client;
using System.Net.Sockets;
using Communication;

namespace Cliente;

internal class Program
{
    static bool isLogged = false;
    static bool serverResponseReceived = false;
    static readonly SettingsManager settingsMngr = new SettingsManager();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Client Application...");
        bool isConnected = false;
        TcpClient? tcpClient = null;
        try
        {
            string ipServer = await settingsMngr.ReadSettings(ClientConfig.serverIPconfigkey);
            int serverPort = int.Parse(await settingsMngr.ReadSettings(ClientConfig.serverPortconfigkey));
            tcpClient = new TcpClient(ipServer, serverPort);
            Console.WriteLine("Client connected to the server!");
            isConnected = true;
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Error connecting to the server: " + ex.Message);
        }

        if (!isConnected)
        {
            Console.WriteLine("Exiting the application because connection to the server failed.");
            return;
        }

        SocketHelper socketHelper = new SocketHelper(tcpClient!);
        FileCommunicationHandler fileHandler = new FileCommunicationHandler(tcpClient!);

        MainMenu mainMenu = new MainMenu(socketHelper);
        LoggedInMenu loggedInMenu = new LoggedInMenu(socketHelper, tcpClient!);

        MenuCommands.Initialize(loggedInMenu, mainMenu, tcpClient!);

        bool exit = false;
        bool isFirstMenuDisplay = true;

        _ = Task.Run(async () =>
        {
            while (tcpClient!.Connected && !exit)
            {
                try
                {
                    string serverMessage = await Task.Run(() => socketHelper.ReceiveMessage());
                    HandleReceiveFile(serverMessage, fileHandler);
                    HandleLoggedIn(serverMessage, socketHelper);
                    serverResponseReceived = true;
                }
                catch (SocketException)
                {
                }
                catch (EndOfStreamException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected error: " + ex.Message);
                }
            }
        });

        while (!exit)
        {
            if (!isFirstMenuDisplay)
            {
                while (!serverResponseReceived)
                {
                    await Task.Delay(100);
                }

                serverResponseReceived = false;
            }

            int option = isLogged ? await loggedInMenu.DisplayLoggedInMenu() : await mainMenu.DisplayMenu();
            exit = await HandleMenuOption(option);

            isFirstMenuDisplay = false;
        }

        socketHelper.CloseConnection();
    }

    static async Task<bool> HandleMenuOption(int option)
    {
        Func<Task>? function = null;
        if (!isLogged)
        {
            MenuCommands.MainMenuCommands!.TryGetValue(option.ToString(), out function);
        }
        else
        {
            MenuCommands.LoggedInMenuCommands!.TryGetValue(option.ToString(), out function);
        }
        if (function != null)
        {
            await function();
        }
        return HandleExitOption(option);
    }

    static bool HandleExitOption(int option)
    {
        if (!isLogged && option == 3)
        {
            return true;
        }
        else if (isLogged && option == 10)
        {
            isLogged = false;
            serverResponseReceived = true;
        }
        return false;
    }

    static void HandleLoggedIn(string serverMessage, SocketHelper socketHelper)
    {
        if (serverMessage.Contains("LOGIN_SUCCESS"))
        {
            isLogged = true;
            Console.WriteLine("\nUsuario logueado correctamente!\n");
        }
        else if (serverMessage.Contains("LOGOUT_SUCCESS"))
        {
            isLogged = false;
            Console.WriteLine("\nUsuario deslogueado correctamente!\n");
        }
        else if (serverMessage.Contains("GAME_PUBLISHED") || serverMessage.Contains("GAME_PURCHASED") ||
                 serverMessage.Contains("GAME_MODIFIED") || serverMessage.Contains("GAME_DELETED") ||
                 serverMessage.Contains("Title") || serverMessage.Contains("Score"))
        {
            Console.WriteLine($"\nDatos del juego: {serverMessage}\n");
        }
        else if (serverMessage.Contains("RATE_ADD"))
        {
            Console.WriteLine("Juego calificado correctamente!");
        }
        else if (serverMessage.Contains("Server is closing."))
        {
            Console.WriteLine("\nServer is shutting down. Closing client...");
            socketHelper.CloseConnectionShutdown();
        }
        else
        {
            Console.WriteLine("\n" + serverMessage + "\n");
        }
    }

    static void HandleReceiveFile(string serverMessage, FileCommunicationHandler fileHandler)
    {
        if (serverMessage.Contains("Enviando archivo"))
        {
            try
            {
                Task.Run(() => fileHandler.ReceiveFile());
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("La imagen que intento ser enviada no se encontro");
            }
        }
    }
}