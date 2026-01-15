namespace Client.Domain
{
    public class Exceptions
    {
        public static async Task<int> GetValidatedMenuOption(string prompt, int min = int.MinValue, int max = int.MaxValue)
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

                // Simulación de asincronía
                await Task.Yield();
            }
            return result;
        }
    }
}