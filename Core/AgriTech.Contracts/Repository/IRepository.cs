

namespace AgriTech;
public interface IRepository : IGetRepository, IDeleteRepository, IPatchRepository, IUpdateRepository, ICreateRepository, IBulkRepository
{
    public const int MaxItemCount = 50;
    void SetContainerName(string containerName);


}


public interface IBulkRepository
{
    Task BulkOperateAsync<TEntity>(IAsyncEnumerable<TEntity> entities, CosmosOperations cosmosOperations,
        CancellationToken cancellationToken) where TEntity : BaseDto;
    Task BulkOperateAsync<TEntity>(IAsyncEnumerable<(string, TEntity)> entities, CosmosOperations cosmosOperations,
    CancellationToken cancellationToken) where TEntity : BaseDto;
}

public enum CosmosOperations
{
    Delete,
    Update,
    Upsert,
    Add
}