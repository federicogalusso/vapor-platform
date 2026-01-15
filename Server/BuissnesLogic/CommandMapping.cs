namespace Server.BuissnesLogic
{
    public class CommandMapping
    {
        public static Dictionary<string, Func<string[], object>>? UserCommands { get; private set; }
        public static Dictionary<string, Func<string[], object>>? GameCommands { get; private set; }

        public static void Initialize(UserLogic userLogic, GameLogic gameLogic, RateLogic rateLogic)
        {
            UserCommands = new Dictionary<string, Func<string[], object>>
            {
                { "CREATE", (args) =>  userLogic.CreateUser(args) },
                { "LOGIN", (args) =>  userLogic.LoginUser(args) }
            };

            GameCommands = new Dictionary<string, Func<string[], object>>
            {
                { "PUBLISH", (args) =>  gameLogic.PublishGame(args) },
                { "PURCHASE", (args) =>  gameLogic.PurchaseGame(args) },
                { "MODIFY", (args) =>  gameLogic.ModifyGame(args) },
                { "DELETE", (args) =>  gameLogic.DeleteGame(args) },
                { "SEARCH", (args) =>  gameLogic.GetGameByTitleString(args) },
                { "RATE", (args) =>  rateLogic.AddRate(args) },
                { "ALL", (args) =>  gameLogic.GetAllGames() },
                { "GENRE", (args) =>  gameLogic.GetGamesByGenre(args) },
                { "PLATFORM", (args) =>  gameLogic.GetGamesByPlatform(args) }
            };
        }
    }
}