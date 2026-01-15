namespace Entities
{
    public interface Event
    {
        EventType EventType { get; set; }
        string UserId { get; set; }
        DateTime CreatedAt { get; set; }
    }
}