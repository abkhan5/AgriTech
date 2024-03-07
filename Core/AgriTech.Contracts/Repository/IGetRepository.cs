

namespace AgriTech;

public interface IGetRepository
{
    Task<TEntity> Get<TEntity>(string id, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task<TEntity> Get<TEntity>(string id, string partitionKey, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task<TEntity> GetItem<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) where TEntity : BaseDto;
    IAsyncEnumerable<TEntity> GetOrderedBy<TEntity, TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDescending, CancellationToken cancellationToken) where TEntity : BaseDto;
    IAsyncEnumerable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken) where TEntity : BaseDto;

    IAsyncEnumerable<TEntity> Get<TEntity, TKey>(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TKey>> keySelector, bool isDescending, CancellationToken cancellationToken)
        where TEntity : BaseDto;
    IAsyncEnumerable<TEntity> GetAll<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto;

    IAsyncEnumerable<TEntity> GetAll<TEntity>(string query, CancellationToken cancellationToken) where TEntity : BaseDto;

    IAsyncEnumerable<TEntity> GetPaged<TEntity>(PagedRequestRecords<TEntity> pagedRecords, string query,
        string orderSequence, CancellationToken cancellationToken, int maxItem = 100) where TEntity : BaseDto;

    IAsyncEnumerable<TEntity> GetEntities<TEntity>(string query, CancellationToken cancellationToken);

    Task<int> GetTotalCount<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task<int> GetTotalCount<TEntity>(CancellationToken cancellationToken) where TEntity : BaseDto;

    Task<double> GetSum(string queryFilter, CancellationToken cancellationToken);
}
