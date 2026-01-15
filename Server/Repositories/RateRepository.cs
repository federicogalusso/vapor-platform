using Entities;
using Interfaces.Repositories;

namespace Server.Repositories
{
    public class RateRepository : IRateRepository
    {
        private readonly List<Rate> _rates = new();

        public async Task Add(string gameTitle, Rate rating)
        {
            await Task.Run(() => _rates.Add(rating));
        }

        public async Task<IEnumerable<Rate>?> GetRatesByGameTitle(string gameTitle)
        {
            return await Task.Run(() => _rates.Where(r => r.GameName == gameTitle));
        }

        public async Task<IEnumerable<Rate>> GetAllRates()
        {
            return await Task.Run(() => _rates);
        }

        public async Task<IEnumerable<Rate>> GetAll(string gameTitle)
        {
            return await Task.Run(() => _rates.Where(r => r.GameName == gameTitle));
        }
    }
}