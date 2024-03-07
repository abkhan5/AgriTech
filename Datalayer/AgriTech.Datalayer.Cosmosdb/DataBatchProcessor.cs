
namespace AgriTech;

internal class DataBatchProcessor(IOptions<AzureCosmosDbSettings> cosmosDbOptions, ILogger<DataBatchProcessor> logger, ApplicationProfile applicationProfile) : IDataBatchProcessor
{
    private readonly IOptions<AzureCosmosDbSettings> cosmosDbOptions = cosmosDbOptions;
    private ChangeFeedProcessor? changeFeedProcessor;
    private readonly ApplicationProfile applicationProfile = applicationProfile;
    private readonly ILogger logger = logger;

    public async Task InitializeChangeFeed<T>(string sourceContainerName, string leaseContainerName,
        string serviceName,
        Func<IReadOnlyCollection<T>, CancellationToken, Task> handleChangesAsync, DateTime? startDateTime,
        CancellationToken cancellationToken)
    {
        var configuration = cosmosDbOptions.Value;
        var databaseName = configuration.DatabaseName;
        var cosmosClient = await CosmosClientFactory.GetNewtonCosmosDbClientOptions(true, applicationProfile.AppName, cosmosDbOptions, cancellationToken, sourceContainerName);
        var leaseContainer = await CreateLeaseContainer(cosmosClient, leaseContainerName);
        var feedConfig = cosmosClient.GetContainer(databaseName, sourceContainerName)
            .GetChangeFeedProcessorBuilder<T>(leaseContainerName,
                async (changes, token) => await handleChangesAsync(changes, cancellationToken))
            .WithInstanceName(serviceName)
            .WithErrorNotification(OnErrorMethod)
            .WithLeaseContainer(leaseContainer);
        if (startDateTime.HasValue)
            if (startDateTime.Value == DateTime.MinValue)
                feedConfig = feedConfig.WithStartTime(DateTime.MinValue.ToUniversalTime());
            else
                feedConfig = feedConfig.WithStartTime(startDateTime.Value);
        changeFeedProcessor = feedConfig.Build();
    }

    public async Task Start()
    => await changeFeedProcessor.StartAsync();


    public async Task Stop()
    => await changeFeedProcessor.StopAsync();


    private Task OnErrorMethod(string leaseToken, Exception exception)
    {
        logger.LogCritical(exception.ToString());
        return Task.CompletedTask;
    }

    private async Task<Container> CreateLeaseContainer(CosmosClient cosmosClient, string leaseContainerName)
    {
        var databaseName = cosmosDbOptions.Value.DatabaseName;
        var db = cosmosClient.GetDatabase(databaseName);
        var leaseContainerProperties = new ContainerProperties(leaseContainerName, "/" + AgriTechConstants.EntityId);
        Container leaseContainer = await db.CreateContainerIfNotExistsAsync(leaseContainerProperties);
        return leaseContainer;
    }
}