namespace AgriTech;

public class CosmosStreamRepository : CosmosBaseRepository, IRepositoryStream
{
    public CosmosStreamRepository(IOptions<AzureCosmosDbSettings> cosmosDbOptions,
        CosmosClientFactory cosmosClientFactory) : base(cosmosDbOptions, cosmosClientFactory)
    {
    }

    public async Task<string> GetDocument(string id, CancellationToken cancellationToken)
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        using var response =
            await container.ReadItemStreamAsync(id, new PartitionKey(id), cancellationToken: cancellationToken);
        if (!response.IsSuccessStatusCode)
            //Handle and log exception
            return null;

        //Read or do other operations with the stream
        using StreamReader streamReader = new(response.Content);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    protected static QueryDefinition GetDescriptorQueryDefinitation(string queryFilter)
    {
        var queryDef = new QueryDefinition(queryFilter);
        return queryDef;
    }

    private static Stream ToStream(string input)
    {
        var streamPayload = new MemoryStream();
        using (var streamWriter = new StreamWriter(streamPayload, Encoding.Default, 1024, true))
        {
            using JsonWriter writer = new JsonTextWriter(streamWriter);
            writer.Formatting = Formatting.None;
            writer.Flush();
            streamWriter.Flush();
        }

        streamPayload.Position = 0;
        return streamPayload;
    }

    public async IAsyncEnumerable<string> GetPagedDocuments(string queryFilter, string continuationToken,
        CancellationToken cancellationToken)
    {
        var container = await GetCosmosContainer(false, cancellationToken);

        using var response = container.GetItemQueryStreamIterator(GetDescriptorQueryDefinitation(queryFilter),
            continuationToken, new QueryRequestOptions
            {
                MaxItemCount = 50,
                MaxConcurrency = -1
            });

        do
        {
            var items = await response.ReadNextAsync(cancellationToken);
            //Read or do other operations with the stream
            using StreamReader streamReader = new(items.Content);
            yield return await streamReader.ReadToEndAsync(cancellationToken);
        } while (response.HasMoreResults);
    }
}