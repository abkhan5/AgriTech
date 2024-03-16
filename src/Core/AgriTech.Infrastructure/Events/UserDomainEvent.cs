namespace AgriTech.Infrastructure.Events;

public sealed record UserDomainEvent : IDomainEvent
{
    public UserDomainEvent(string eventName, string message, string userId)
    {
        UserEvent = new UserEventDto
        {
            EventName = eventName,
            Message = message,
            DeviceId = userId
        };
    }



    public UserEventDto UserEvent { get; }
}