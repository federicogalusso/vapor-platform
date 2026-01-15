using System.Net.Sockets;

namespace Client
{
    public class MenuCommands
    {
        public static Dictionary<string, Func<Task>>? MainMenuCommands { get; private set; }
        public static Dictionary<string, Func<Task>>? LoggedInMenuCommands { get; private set; }

        public static void Initialize(LoggedInMenu loggedInMenu, MainMenu mainMenu, TcpClient tcpClient)
        {
            MainMenuCommands = new Dictionary<string, Func<Task>>
            {
                { "1", new Func<Task>(() => mainMenu.CreateUser()) },
                { "2", new Func<Task>(() => mainMenu.LoginUser()) }
            };

            LoggedInMenuCommands = new Dictionary<string, Func<Task>>
            {
                { "1", new Func<Task>(() => loggedInMenu.PublishGame()) },
                { "2", new Func<Task>(() => loggedInMenu.PurchaseGame()) },
                { "3", new Func<Task>(() => loggedInMenu.EditGame()) },
                { "4", new Func<Task>(() => loggedInMenu.DeleteGame()) },
                { "5", new Func<Task>(() => loggedInMenu.SearchGame()) },
                { "6", new Func<Task>(() => loggedInMenu.RateGame()) },
                { "7", new Func<Task>(() => loggedInMenu.ListAllGames()) },
                { "8", new Func<Task>(() => loggedInMenu.ListGamesByGenre()) },
                { "9", new Func<Task>(() => loggedInMenu.ListGamesByPlatform()) }
            };
        }
    }
}