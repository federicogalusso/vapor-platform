namespace Entities;

public class UserEvent
{
    public string EventName { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserEvent()
    {
    }
}