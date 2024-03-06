namespace AgriTech.Contracts;

public interface IMessageReceiver
{
    public string QueueName { get; }
    Task OnMessageAsync(string queueName, Func<CancellationToken, Task> callBack, CancellationToken token);
    //   Task OnMessageAsync<T>(string queueName, Func<T, Task> callBack, CancellationToken token);
    Task OnMessageAsync<T>(string queueName, Func<T, CancellationToken, Task> callBack, CancellationToken token);

    Task Stop(CancellationToken token);
}