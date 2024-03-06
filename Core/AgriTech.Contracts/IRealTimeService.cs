namespace AgriTech.Contracts;

public interface IRealTimeService
{
    Task SendMessage(string hubMethod, object?[] payload, CancellationToken cancellationToken);
    Task StreamMessages<TEntity>(string hubMethod, Func<IAsyncEnumerable<TEntity>> stream, CancellationToken cancellationToken);

}
