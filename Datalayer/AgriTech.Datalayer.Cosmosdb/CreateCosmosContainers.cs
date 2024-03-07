using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AgriTech;

public class CosmosOpsContainers
{
    private readonly IOptions<AzureCosmosDbSettings> cosmosDbOptions;

    public CosmosOpsContainers(IOptions<AzureCosmosDbSettings> cosmosDbOptions)
    {
        this.cosmosDbOptions = cosmosDbOptions;
    }

    public async Task CreateDbs()
    {
        var factory = new CosmosClientFactory(new ApplicationProfile { AppName = "Test" });
        var client = await factory.CreateClientAsync(true, cosmosDbOptions, CancellationToken.None);
        var containerNames = cosmosDbOptions.Value.GetContainerNames();
        var db = client.GetDatabase(cosmosDbOptions.Value.DatabaseName);

        var throughput = ThroughputProperties.CreateManualThroughput(400);
        //await db.DeleteAsync();
        //client.CreateDatabaseAsync()
        foreach (var item in containerNames)
            await CreateContainer(db, item, "/id");
    }

    private async Task CreateContainer(Database database, string containerName, string partitionKey)
    {
        ContainerProperties containerProperties = new()
        {
            Id = containerName,
            PartitionKeyPath = partitionKey
        };

        Container container = await database.CreateContainerIfNotExistsAsync(containerProperties);
        Console.Write($"-----> Deleting container {containerName} with Partition Key {partitionKey} ..... ");
        await container.DeleteContainerAsync();
        Console.Write($"-----> Creating container {containerName} with Partition Key {partitionKey} ");
        await database.CreateContainerIfNotExistsAsync(containerProperties);
        Console.WriteLine();
    }
}