using Server.Services;
using Exceptions;
using Interfaces.Services;

namespace Server.BuissnesLogic
{
    public class RateLogic
    {
        private readonly IRateService service;
        private readonly object _lock = new object();

        public RateLogic(RateService service)
        {
            this.service = service;
        }

        public string AddRate(string[] param)
        {
            string gameTitle = param[0];
            if (!int.TryParse(param[1], out int score))
            {
                return "Error: La puntuacion debe ser un numero";
            }
            string comment = param[2];

            string result;
            lock (_lock)
            {
                try
                {
                    result = service.AddRate(gameTitle, score, comment).Result;
                }
                catch (DomainException ex)
                {
                    return $"Error: {ex.Message}";
                }
                catch (LogicException ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
            return result;
        }
    }
}