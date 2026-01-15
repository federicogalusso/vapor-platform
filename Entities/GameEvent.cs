namespace Entities
{
    public class GameEvent
    {
        public string EventType { get; set; }
        public string Publisher { get; set; }
        public string Title { get; set; }
        public string? Type { get; set; }
        public string? LaunchDate { get; set; }
        public string? Platform { get; set; }
        public string? Image { get; set; }
        public string? AvailableUnits { get; set; }
        public string? Owner { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"Publisher: {Publisher}, Title: {Title}, Type: {Type}, LaunchDate: {LaunchDate}, Platform: {Platform}, Image: {Image}, AvailableUnits: {AvailableUnits}, Owner: {Owner}, IpAddress: {IpAddress}";
        }

        public GameEvent()
        {
            EventType = "";
            Publisher = "";
            Title = "";
            Type = "";
            LaunchDate = "";
            Platform = "";
            Image = "";
            AvailableUnits = "";
            Owner = "";
            IpAddress = "";
        }
    }
}