
namespace AgriTech;

public interface IUpdateRepository
{
    Task Update<TEntity>(TEntity entity, string partitionKey, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : BaseDto;

    Task Upsert<TEntity>(TEntity entity, string partitionKey, CancellationToken cancellationToken = default) where TEntity : BaseDto;

    Task Upsert<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : BaseDto;

    Task Upsert<TEntity>(TEntity entity, List<string> partitionKeys, CancellationToken cancellationToken = default) where TEntity : BaseDto;
}
