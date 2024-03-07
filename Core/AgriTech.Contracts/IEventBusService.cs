namespace AgriTech;

public interface IEventBusService
{

    Task Subscribe<T>(string topicName, string subscriptionName, bool toForward, Func<T, CancellationToken, Task<bool>> callBack, CancellationToken cancellationToken);
    Task Subscribe<T>(string topicName, string subscriptionName, bool toForward, Func<T, CancellationToken, Task<bool>> callBack, IDictionary<string, string> traits, CancellationToken cancellationToken);

    Task Publish<T>(T domainEvent, string topicName, CancellationToken token);
    Task Publish<T>(T domainEvent, string topicName, IDictionary<string, string> traits, CancellationToken token);
}
