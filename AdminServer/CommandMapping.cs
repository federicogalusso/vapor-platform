namespace AdminServer;

public class CommandMapping
{
    public static Dictionary<string, Func<Task>>? Commands { get; private set; }

    public static void Initialize(Menu menu)
    {
        Commands = new Dictionary<string, Func<Task>>()
        {
            { "1", (menu.PublishGame) },
            { "2", (menu.ModifyGame) },
            { "3", (menu.DeleteGame) },
            { "4", (menu.GetGameRating) },
            { "5", (menu.GetNextPurchases) },
            { "6", menu.Exit }
        };
    }

}
