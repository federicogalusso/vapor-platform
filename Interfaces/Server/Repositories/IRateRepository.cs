using Entities;

namespace Interfaces.Repositories
{
    public interface IRateRepository
    {
        Task Add(string gameTitle, Rate rating);
        Task<IEnumerable<Rate>?> GetRatesByGameTitle(string gameTitle);
        Task<IEnumerable<Rate>> GetAllRates();
    }
}