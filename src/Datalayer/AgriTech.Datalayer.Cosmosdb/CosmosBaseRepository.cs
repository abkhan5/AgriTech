
namespace AgriTech;
public abstract class CosmosBaseRepository
{
    protected const int MaxItemCount = 25;
    private readonly CosmosClientFactory cosmosClientFactory;
    private readonly IOptions<AzureCosmosDbSettings> cosmosDbOptions;
    private string containerName;
    private Container cosmosContainer;

    public CosmosBaseRepository(IOptions<AzureCosmosDbSettings> cosmosDbOptions,
        CosmosClientFactory cosmosClientFactory)
    {
        this.cosmosDbOptions = cosmosDbOptions;
        this.cosmosClientFactory = cosmosClientFactory;
    }


    protected async Task<Container> GetCosmosContainer(bool allowBulkExecution = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(containerName))
            containerName = cosmosDbOptions.Value.ContainerName;

        if (string.IsNullOrEmpty(containerName))
            throw new Exception("Container name is null");

        if (cosmosContainer != null)
            return cosmosContainer;

        var cosmosClient =
            await cosmosClientFactory.CreateClientAsync(allowBulkExecution, cosmosDbOptions, cancellationToken);
        cosmosContainer = cosmosClient.GetContainer(cosmosDbOptions.Value.DatabaseName, containerName);
        return cosmosContainer;
    }

    public void SetContainerName(string containerName)
    {
        this.containerName = containerName;
        cosmosContainer = null;
    }

    protected static QueryDefinition GetDescriptorQueryDefinitation<TEntity>(string queryFilter = null)
    {
        string query;
        var instance = Activator.CreateInstance<TEntity>();
        var entityDiscriminator = typeof(TEntity).GetProperty("Discriminator").GetValue(instance);
        var entityName = entityDiscriminator.ToString();
        if (string.IsNullOrEmpty(queryFilter) || queryFilter.StartsWith("Order"))
            query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM root r WHERE r.Discriminator='{0}' ",
                entityName);
        else
            query = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM root r WHERE r.Discriminator='{0}' ",
                entityName) + queryFilter;
        var queryDef = new QueryDefinition(query);
        return queryDef;
    }

    protected static QueryDefinition GetCountQueryDefinitation<TEntity>(string queryFilter = null) where TEntity : BaseDto
    {
        var entityName = (Activator.CreateInstance<TEntity>()).Discriminator;
        string query;
        if (string.IsNullOrEmpty(queryFilter) || queryFilter.StartsWith("Order"))
            query = string.Format(CultureInfo.InvariantCulture,
                "SELECT value count(1) FROM root r WHERE r.Discriminator='{0}' ", entityName);
        else
            query = string.Format(CultureInfo.InvariantCulture,
                "SELECT value count(1) FROM root r WHERE r.Discriminator='{0}' ", entityName) + queryFilter;

        var queryDef = new QueryDefinition(query);
        return queryDef;
    }
}