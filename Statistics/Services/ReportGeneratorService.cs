using Entities;
using Interfaces.Statistics.Services;

namespace WebApiRabbitMQ.Service
{
    public class ReportGeneratorService
    {
        public static async Task<Dictionary<string, int>> GetSalesByPublisher(List<GameEvent> gamesEvents)
        {
            Dictionary<string, int> ventasPorPublicador = new Dictionary<string, int>();
            foreach (var gameEvent in gamesEvents)
            {
                if (gameEvent.EventType == "Purchase")
                {
                    await Task.Delay(5000);
                    if (ventasPorPublicador.ContainsKey(gameEvent.Publisher))
                    {
                        ventasPorPublicador[gameEvent.Publisher]++;
                    }
                    else
                    {
                        ventasPorPublicador[gameEvent.Publisher] = 1;
                    }
                }
            }
            return ventasPorPublicador;
        }
    }
}