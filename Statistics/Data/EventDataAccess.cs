
using Entities;

namespace Statistics.Data
{
    public class EventDataAccess
    {
        private List<Event> statistics;
        private readonly object padlock;
        private static EventDataAccess? instance;
        private static readonly object singletonPadlock = new();
        public static EventDataAccess GetInstance()
        {

            lock (singletonPadlock)
            {
                instance ??= new EventDataAccess();
            }
            return instance;
        }

        public EventDataAccess()
        {
            statistics = [];
            padlock = new object();
        }

        public void AddStatistics(Event forecast)
        {
            lock (padlock)
            {
                statistics.Add(forecast);
            }
        }

        public Event[] Getstatistics()
        {
            lock (padlock)
            {
                return [.. statistics];
            }
        }

    }
}