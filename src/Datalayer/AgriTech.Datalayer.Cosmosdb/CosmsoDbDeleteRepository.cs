namespace AgriTech;

public partial class CosmosDbSqlRepository : CosmosBaseRepository, IRepository
{
    public async Task Delete(string id, string partitionKey, CancellationToken cancellationToken)
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        await container.DeleteItemStreamAsync(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
        //await container.DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKey),cancellationToken: cancellationToken);
    }

    public Task DeleteAll<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto =>
         BulkOperateAsync(GetAll<TEntity>(cancellationToken), CosmosOperations.Delete, cancellationToken);
    public async Task DeleteFor(string partitionKey, CancellationToken cancellationToken)
    {
        List<Task> concurrentTasks = [];
        var container = await GetCosmosContainer(false, cancellationToken);
        //ResponseMessage deleteResponse = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(partitionKey));

        using var feedIterator = container.GetItemQueryIterator<CosmosRecord>(
           requestOptions: new QueryRequestOptions
           {
               PartitionKey = new PartitionKey(partitionKey),
               MaxConcurrency = -1
           });


        while (feedIterator.HasMoreResults)
        {
            var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
            foreach (var responseItem in feedResponse)
            {
                concurrentTasks.Add(container.DeleteItemStreamAsync(responseItem.Id, new PartitionKey(partitionKey), cancellationToken: cancellationToken));
                if (concurrentTasks.Count > 50)
                {
                    await Task.WhenAll(concurrentTasks);
                    await Task.Delay(1000, cancellationToken);
                    concurrentTasks.Clear();
                }
            }
        }
        await Task.WhenAll(concurrentTasks);
    }

    public async Task Delete<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        var record = await Get<TEntity>(entity.Id, cancellationToken);
        if (record != null)
            await container.DeleteItemAsync<TEntity>(entity.Id, new PartitionKey(entity.Id), cancellationToken: cancellationToken);
    }

}