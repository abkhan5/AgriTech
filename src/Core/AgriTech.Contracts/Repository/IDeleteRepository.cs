namespace AgriTech;

public interface IDeleteRepository
{
    Task Delete(string id, string partitionKey, CancellationToken cancellationToken);
    Task DeleteFor(string partitionKey, CancellationToken cancellationToken);
    Task DeleteAll<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Delete<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : BaseDto;
}
