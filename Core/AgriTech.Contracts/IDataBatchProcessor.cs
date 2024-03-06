namespace AgriTech.Contracts;
public interface IDataBatchProcessor
{
    Task InitializeChangeFeed<T>(string sourceContainerName, string leaseContainerName, string serviceName,
        Func<IReadOnlyCollection<T>, CancellationToken, Task> handleChangesAsync, DateTime? startDateTime,
        CancellationToken cancellationToken);

    Task Start();
    Task Stop();
}
