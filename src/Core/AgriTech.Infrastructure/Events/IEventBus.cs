using System.Text.Json.Serialization;

namespace AgriTech.Infrastructure.Events;

public interface IEventBus
{
    Task Publish(IntegrationEvent @event, CancellationToken cancellationToken);

    Task Initialize(CancellationToken cancellationToken);

    Task Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    //Task Unsubscribe<T, TH>()
    //    where TH : IIntegrationEventHandler<T>
    //    where T : IntegrationEvent;
}

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonPropertyName("Id")] public Guid Id { get; }

    [JsonPropertyName("CreationDate")] public DateTime CreationDate { get; }
}