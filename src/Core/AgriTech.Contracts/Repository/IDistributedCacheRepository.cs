namespace AgriTech;

public interface IDistributedCacheRepository
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken, bool checkLocal = true);
    Task SetEntityAsync<T>(string key, T entity, TimeSpan expiryTime, CancellationToken cancellationToken, bool setLocal = true);
    Task RemoveAll(string key, CancellationToken cancellationToken);
    Task RemoveAll(IEnumerable<string> key, CancellationToken cancellationToken);
    Task RefreshCache(string key, CancellationToken cancellationToken);
    //Task RemoveKeys(string keys, CancellationToken cancellationToken);
}