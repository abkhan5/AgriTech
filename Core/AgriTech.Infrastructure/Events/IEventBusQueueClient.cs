namespace AgriTech.Infrastructure.Events;

public interface IEventBusQueueClient : IDisposable
{
    string Path { get; }
    void OnMessageAsync<T>(Func<T, Task> callBack, Action onException, int maxConcurrentCall);
    Task SendAsync<T>(string queueName, IEnumerable<T> messages);
    Task SendAsync<T>(string queueName, T message);
    Task CloseAsync();
}