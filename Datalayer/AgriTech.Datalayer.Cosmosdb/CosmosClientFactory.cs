namespace AgriTech;

public class CosmosClientFactory
{
    private readonly ApplicationProfile applicationProfile;
    private CosmosClient cosmosClient;

    public CosmosClientFactory(ApplicationProfile applicationProfile)
    {
        this.applicationProfile = applicationProfile;
    }   
    public async Task<CosmosClient> CreateClientAsync(bool allowBulkExecution,
        IOptions<AzureCosmosDbSettings> cosmosDbOptions, CancellationToken cancellationToken)
    {
        if (cosmosClient == null)
        {
            var connectionString =
                $"AccountEndpoint={cosmosDbOptions.Value.Account};AccountKey={cosmosDbOptions.Value.Key}";
            List<(string databaseId, string containerId)> containers = new();
            var containerNames = cosmosDbOptions.Value.GetContainerNames();
            foreach (var containerName in containerNames)
                containers.Add((cosmosDbOptions.Value.DatabaseName, containerName));
            var appName = applicationProfile.AppName;

            cosmosClient = await CosmosClient.CreateAndInitializeAsync(connectionString, containers,
                GetCosmosDbClientOptions(allowBulkExecution, appName), cancellationToken);
        }

        return cosmosClient;
    }

    public static async Task<CosmosClient> GetNewtonCosmosDbClientOptions(bool allowBulkExecution, string appName,
        IOptions<AzureCosmosDbSettings> cosmosDbOptions, CancellationToken cancellationToken,
        params string[] containerNames)
    {
        var connectionString = $"AccountEndpoint={cosmosDbOptions.Value.Account};AccountKey={cosmosDbOptions.Value.Key}";
        List<(string databaseId, string containerId)> containers = [];
        foreach (var containerName in containerNames)
            containers.Add((cosmosDbOptions.Value.DatabaseName, containerName));

        return await CosmosClient.CreateAndInitializeAsync(connectionString, containers,
            GetNewtonCosmosDbClientOptions(allowBulkExecution, appName), cancellationToken);
    }

    public static CosmosClientOptions GetNewtonCosmosDbClientOptions(bool allowBulkExecution, string appName)
    => new()
    {
        MaxRetryAttemptsOnRateLimitedRequests = 5,
        MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(2.4),
        AllowBulkExecution = allowBulkExecution,
        ConnectionMode = ConnectionMode.Direct,
        ApplicationName = appName,
        EnableContentResponseOnWrite = false
    };

    public static CosmosClientOptions GetCosmosDbClientOptions(bool allowBulkExecution, string appName)
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        CosmosSystemTextJsonSerializer cosmosSystemTextJsonSerializer = new(jsonSerializerOptions);
        return new CosmosClientOptions
        {
            MaxRetryAttemptsOnRateLimitedRequests = 5,
            MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(2.4),
            AllowBulkExecution = allowBulkExecution,
            ConnectionMode = ConnectionMode.Direct,
            ApplicationName = appName,
            Serializer = cosmosSystemTextJsonSerializer,
            EnableContentResponseOnWrite = false
        };
    }
}