using Interfaces.Repositories;
using Entities;
using Exceptions;
using Interfaces.Services;

namespace Server.Services
{
    public class RateService : IRateService
    {
        private readonly IRateRepository _rateRepository;
        private readonly IGameRepository _gameRepository;

        public RateService(IRateRepository rateRepository, IGameRepository gameRepository)
        {
            _rateRepository = rateRepository;
            _gameRepository = gameRepository;
        }

        public async Task<string> AddRate(string gameTitle, int score, string comment)
        {
            if (await DoesGameExist(gameTitle))
            {
                Rate rate = new Rate
                {
                    GameName = gameTitle,
                    Score = score,
                    Comment = comment
                };
                await _rateRepository.Add(gameTitle, rate);
                return "RATE_ADDED";
            }
            else
            {
                throw new LogicException("No existe un juego con el titulo ingresado.");
            }
        }

        public async Task<bool> DoesGameExist(string title)
        {
            var existingGame = await _gameRepository.GetByTitle(title);
            return existingGame != null;
        }
    }
}