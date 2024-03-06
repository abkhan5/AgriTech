
namespace AgriTech.Workers;

public abstract class CosmosChangeFeedWorker : BackgroundService
{
    protected readonly IDataBatchProcessor dataBatchProcessor;
    private readonly string leaseContainerName;
    protected readonly ILogger logger;
    private readonly string serviceName;
    private readonly string sourceContainerName;
    protected DateTime? startDateTime;

    public CosmosChangeFeedWorker(ILogger logger, IDataBatchProcessor dataBatchProcessor, string containerName,
        string leaseContainerName, string serviceName)
    {
        this.logger = logger;
        this.dataBatchProcessor = dataBatchProcessor;
        sourceContainerName = containerName;
        this.leaseContainerName = leaseContainerName;
        this.serviceName = serviceName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await dataBatchProcessor.InitializeChangeFeed<JObject>(sourceContainerName,
            leaseContainerName, serviceName,
            HandleChangesAsync, startDateTime, stoppingToken);
        await SetUpWorker(stoppingToken);
        await dataBatchProcessor.Start();
    }


    protected virtual Task SetUpWorker(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await dataBatchProcessor.Stop();
    }

    protected abstract Task HandleChangesAsync(IReadOnlyCollection<JObject> changes,
        CancellationToken cancellationToken);
}