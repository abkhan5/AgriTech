namespace AgriTech.Contracts;
public interface IDataBatchProcessor
{
    Task InitializeChangeFeed<T>(string sourceContainerName, string leaseContainerName, string serviceName,
        Func<IReadOnlyCollection<T>, CancellationToken, Task> handleChangesAsync, DateTime? startDateTime,
        CancellationToken cancellationToken);

    Task Start();
    Task Stop();
}
public static class HubEventNames
{
    public const string ResultReceiver = "receiver";
    public const string Search = "Search";
    public const string SendNotification = "SendNotification";
}
