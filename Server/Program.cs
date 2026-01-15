using System.Net.Sockets;
using System.Net;
using Server.Services;
using Server.BuissnesLogic;
using Communication;
using Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Server;

public class Program
{
    static readonly SettingsManager settingsMngr = new SettingsManager();

    private static IUserRepository userRepository;
    private static IRateRepository rateRepository;
    private static IGameRepository gameRepository;

    private static UserService userService;

    private static GameService gameService;
    private static RateService rateService;

    private static EventPublisher eventPublisher;

    private static GameLogic gameLogic;
    private static UserLogic userLogic;
    private static RateLogic rateLogic;

    private static List<ClientInfo> _users = new List<ClientInfo>();
    private static TcpListener? tcpListener;

    private static List<Task> clientTasks = new List<Task>();
    private static CountdownEvent? shutdownCountdown;

    private static void ConfigureServices(IServiceProvider services)
    {
        userRepository = services.GetRequiredService<IUserRepository>();
        rateRepository = services.GetRequiredService<IRateRepository>();
        gameRepository = services.GetRequiredService<IGameRepository>();

        eventPublisher = services.GetRequiredService<EventPublisher>();

        gameService = new GameService(gameRepository, rateRepository, eventPublisher);
        rateService = new RateService(rateRepository, gameRepository);
        userService = new UserService(userRepository, eventPublisher);
        
        gameLogic = new GameLogic(gameService);
        userLogic = new UserLogic(userService);
        rateLogic = new RateLogic(rateService);
    }

    public static async Task InitTcpServer(IServiceProvider services)
    {
        ConfigureServices(services);
        
        
        Console.WriteLine("Starting Server Application...");

        CommandMapping.Initialize(userLogic, gameLogic, rateLogic);

        var cts = new CancellationTokenSource();
        var shutdownRequested = new TaskCompletionSource<bool>();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            HandleShutdownRequest(cts, shutdownRequested);
        };


        await StartServer(cts.Token);
    }

    static async Task StartServer(CancellationToken cancellationToken)
    {
        string ipServer = await settingsMngr.ReadSettings(ServerConfig.serverIPconfigkey);
        int ipPort = int.Parse(await settingsMngr.ReadSettings(ServerConfig.serverPortconfigkey));
        tcpListener = new TcpListener(IPAddress.Parse(ipServer), ipPort);

        tcpListener.Start();
        Console.WriteLine($"Server is listening on {ipServer}:{ipPort}");

        int i = 1;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (tcpListener.Pending())
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                var clientInfo = new ClientInfo(tcpClient);
                lock (_users)
                {
                    _users.Add(clientInfo);
                }

                Console.WriteLine("Accepted a new connection.");
                int clientNumber = i;

                Task clientTask = HandleClient(clientInfo, cancellationToken);
                i++;
            }
        }
    }

    static void HandleShutdownRequest(CancellationTokenSource cts, TaskCompletionSource<bool> shutdownRequested)
    {
        Task.Run(async () =>
        {
            Console.WriteLine("\nCtrl + C pressed.");

            Console.WriteLine("Do you want to wait for clients to disconnect? (y/n)");
            string? response = Console.ReadLine();

            bool waitForClients = response != null && response.ToLower() == "y";

            lock (_users)
            {
                MarkClientForShutdown();
            }

            if (waitForClients)
            {
                Console.WriteLine("Waiting for clients to disconnect...");
                shutdownCountdown = new CountdownEvent(_users.Count);
                await WaitForClientsToDisconnect();
            }
            else
            {
                Console.WriteLine("Forcing disconnection of clients...");
                ForceDisconnectClient();
            }
            await ShutdownServer(cts);
        });
    }

    static void MarkClientForShutdown()
    {
        foreach (var client in _users)
        {
            client.MarkedForShutdown = true;
        }
    }

    static async Task WaitForClientsToDisconnect()
    {
        if (shutdownCountdown != null)
        {
            await Task.Run(() => shutdownCountdown.Wait());

        }
    }

    static async void ForceDisconnectClient()
    {
        string shutdownMessage = "Server is closing.";

        foreach (var clientInfo in _users)
        {
            try
            {
                SocketHelper socketHelper = new SocketHelper(clientInfo.Client);
                await socketHelper.SendMessage(shutdownMessage);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error sending shutdown message to client: {ex.Message}");
            }
            finally
            {
                clientInfo.Client.Close();
            }
        }

    }

    static async Task HandleClient(ClientInfo clientInfo, CancellationToken cancellationToken)
    {

        bool isConnectionActive = true;
        SocketHelper socketHelper = new SocketHelper(clientInfo.Client);
        FileCommunicationHandler fileHandler = new FileCommunicationHandler(clientInfo.Client);
        while (isConnectionActive)
        {
            try
            {
                string message = await socketHelper.ReceiveMessage();

                Console.WriteLine("Client: " + message);

                if (FileSent(message))
                {
                    await fileHandler.ReceiveFile();
                }

                string response = await ProcessClientMessage(clientInfo, message);

                await socketHelper.SendMessage(response);
                await HandleFileTransfer(message, socketHelper, fileHandler);

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Conexión con el cliente cancelada.");
                await DisconnectUser(clientInfo);
                isConnectionActive = false;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Client disconnected");
                await DisconnectUser(clientInfo);
                isConnectionActive = false;
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FileNotFoundException ex)
            {

                Console.WriteLine("File not found");
            }
            catch (Exception)
            {
                await DisconnectUser(clientInfo);
                isConnectionActive = false;
            }
        }
    }

    static async Task<string> ProcessClientMessage(ClientInfo client, string message)
    {
        Func<string[], object> clientFunction;
        string[] parts = message.Split(':');
        if (parts.Length < 2)
        {
            return "Error: Formato de mensaje inválido.";
        }

        string command = parts[0];
        string[] entity = command.Split('_');
        if (entity[0] == "USER")
        {
            CommandMapping.UserCommands.TryGetValue(entity[1], out clientFunction);
        }
        else
        {
            CommandMapping.GameCommands.TryGetValue(entity[1], out clientFunction);
        }

        if (clientFunction == null)
        {
            return "Error: Comando inválido.";
        }

        string[] parameters = parts[1].Split('-');
        Array.Resize(ref parameters, parameters.Length + 1);
        parameters[^1] = client.Username;

        try
        {
            object result = clientFunction(parameters);
            if (parts[0] == "USER_LOGIN" && (string)result == "LOGIN_SUCCESS")
            {
                client.Username = parameters[0];
            }
            return result.ToString() ?? "";
        }
        catch (Exception ex)
        {
            return $"{ex.Message}";
        }
    }

    static async Task ShutdownServer(CancellationTokenSource cts)
    {
        cts.Cancel();

        ShutdownRabbitMQ();

        tcpListener.Stop();

        _users.Clear();

        Console.WriteLine("Server shutdown complete.");
    }

    static async Task DisconnectUser(ClientInfo clientInfo)
    {
        clientInfo.Client.Close();
        lock (_users)
        {
            _users.Remove(clientInfo);
        }

        if (clientInfo.MarkedForShutdown && shutdownCountdown != null)
        {
            shutdownCountdown.Signal();
        }
    }

    static bool FileSent(string message)
    {
        if (message.Contains("GAME_PUBLISH") || message.Contains("GAME_MODIFY"))
        {
            var filePart = message.Split('-').Last();
            return filePart.Trim().Length > 0;
        }
        return false;
    }

    static async Task HandleFileTransfer(string clientMessage, SocketHelper socketHelper, FileCommunicationHandler fileCommunicationHandler)
    {
        var title = clientMessage.Split(':')[1];

        var game = await gameLogic.GetGameByTitle(title);
        if (clientMessage.Contains("GAME_SEARCH") && game != null)
        {
            var file = await gameLogic.GetGameImage(title);
            if (!string.IsNullOrEmpty(file))
            {
                await socketHelper.SendMessage("Enviando archivo");
                await fileCommunicationHandler.SendFile(file);
            }
        }
    }

    static void ShutdownRabbitMQ()
    {
        if (eventPublisher != null)
        {
            eventPublisher.Close();
            Console.WriteLine("RabbitMQ connection closed.");
        }
    }
}