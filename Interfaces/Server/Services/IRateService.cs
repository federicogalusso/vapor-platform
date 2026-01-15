namespace Interfaces.Services
{
    public interface IRateService
    {
        Task<string> AddRate(string gameTitle, int score, string comment);
        Task<bool> DoesGameExist(string title);
    }
}