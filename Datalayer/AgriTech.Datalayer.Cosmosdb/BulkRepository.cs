

namespace AgriTech;

public partial class CosmosDbSqlRepository : IBulkRepository
{
    #region Bulk Save Entites

    public async Task BulkOperateAsync<TEntity>(IAsyncEnumerable<(string, TEntity)> entities, CosmosOperations cosmosOperations,
        CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        List<Task> concurrentTasks = [];
        await foreach (var entityItem in entities)
        {
            AddOperationTask(concurrentTasks, entityItem, cosmosOperations, container, cancellationToken);
            if (concurrentTasks.Count > 50)
            {
                await Task.WhenAll(concurrentTasks);
                await Task.Delay(1000, cancellationToken);
                concurrentTasks.Clear();
            }
        }

        await Task.WhenAll(concurrentTasks);
    }

    private static void AddOperationTask<TEntity>(List<Task> concurrentTasks, (string, TEntity) entity,
        CosmosOperations cosmosOperations, Container container, CancellationToken cancellation) where TEntity : BaseDto
    {
        var partitionKey = entity.Item1;
        var entityItem = entity.Item2;
        Task operationTask = null;
        switch (cosmosOperations)
        {
            case CosmosOperations.Delete:
                operationTask = container.DeleteItemAsync<TEntity>(entityItem.Id, new PartitionKey(partitionKey),
                    cancellationToken: cancellation);
                break;
            case CosmosOperations.Update:
                operationTask = container.ReplaceItemAsync(entityItem, entityItem.Id, new PartitionKey(partitionKey),
                    cancellationToken: cancellation);
                break;
            case CosmosOperations.Upsert:
                operationTask = container.UpsertItemAsync(entityItem, new PartitionKey(partitionKey),
                    cancellationToken: cancellation);
                break;
            case CosmosOperations.Add:
                operationTask = container.CreateItemAsync(entityItem, new PartitionKey(partitionKey),
                    cancellationToken: cancellation);
                break;
        }

        if (operationTask == null)
            return;

        concurrentTasks.Add(operationTask);
    }

    #endregion
}