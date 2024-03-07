namespace AgriTech;

public interface ICreateRepository
{
    Task Create<TEntity>(TEntity entity, string partitionKey, CancellationToken cancellationToken);
    Task Create<TEntity>(TEntity entity, CancellationToken cancellationToken);

}
