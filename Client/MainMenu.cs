using Communication;

namespace Client
{
    public class MainMenu
    {
        private readonly SocketHelper _socketHelper;

        public MainMenu(SocketHelper socketHelper)
        {
            _socketHelper = socketHelper;
        }

        public async Task<int> DisplayMenu()
        {
            Console.Write(" -------------------------\n" +
                          "|          MENÚ           |" + "\n" +
                          " -------------------------" + "\n" +
                          "|   1 - Crear usuario     |\n" +
                          "|   2 - Ingresar usuario  |\n" +
                          "|   3 - Salir             |" +
                          "\n" + " -------------------------" + "\n\n");

            int option = await Client.Domain.Exceptions.GetValidatedMenuOption("Elija una opción (1-3):", 1, 3);
            return option;
        }

        public async Task CreateUser()
        {
            Console.Write("Ingrese nombre de usuario: ");
            string? username = Console.ReadLine();
            Console.Write("Ingrese contraseña: ");
            string? password = Console.ReadLine();

            string message = $"USER_CREATE:{username}-{password}";
            await Task.Run(() => _socketHelper.SendMessage(message));
        }

        public async Task LoginUser()
        {
            Console.Write("Ingrese nombre de usuario: ");
            string? username = Console.ReadLine();
            Console.Write("Ingrese contraseña: ");
            string? password = Console.ReadLine();

            string message = $"USER_LOGIN:{username}-{password}";
            await Task.Run(() => _socketHelper.SendMessage(message));
        }
    }
}