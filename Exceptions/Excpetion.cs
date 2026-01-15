namespace Exceptions
{
    public class Excpetion
    {
        public static int GetValidatedInt(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out result))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            return result;
        }

        public static string GetValidatedString(string prompt)
        {
            string? input;
            while (true)
            {
                input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    break; // Valid input, exit loop
                }
                else
                {
                    Console.WriteLine(prompt);
                }
            }
            return input;
        }



    }
}