namespace AgriTech.Datalayer.Redis;

public sealed record AzureRedisSettings
{
    public const string AzureRedisOptions = "AzureRedisSettings";
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}

public class RedisCacheRepository(ILogger<RedisCacheRepository> logger, IDistributedCache cache, IMemoryCache memoryCache, IOptions<AzureRedisSettings> options) : IDistributedCacheRepository
{
    private readonly IDistributedCache cache = cache;
    private readonly IMemoryCache memoryCache = memoryCache;
    private readonly ILogger<RedisCacheRepository> logger = logger;
    private readonly IOptions<AzureRedisSettings> options = options;
    private readonly string redConn = options.Value.ConnectionString;

    async Task<T> IDistributedCacheRepository.GetAsync<T>(string key, CancellationToken cancellationToken, bool checkLocal)
    {
        try
        {
            if (checkLocal)
            {
                var cacheData = memoryCache.Get<T>(key);
                if (cacheData != null)
                    return cacheData;
            }

            var cacheRespnse = await cache.GetAsync(key, cancellationToken);
            if (cacheRespnse == null)
                return default;

            var decompressed = (await Encoding.UTF8.GetString(cacheRespnse).Decompress(cancellationToken));
            var response = decompressed.FromJson<T>();
            if (checkLocal)
                memoryCache.Set(key, response, TimeSpan.FromMinutes(10));
            return response;
        }
        catch (Exception e)
        {
            memoryCache.Remove(key);
            await cache.RemoveAsync(key, cancellationToken);
            logger.LogError(e, "Error for key {error}", key);
            return default;
        }
    }

    async Task IDistributedCacheRepository.RemoveAll(string key, CancellationToken cancellationToken)
    {
        memoryCache.Remove(key);
        await cache.RemoveAsync(key, cancellationToken);
    }

    async Task IDistributedCacheRepository.RemoveAll(IEnumerable<string> keys, CancellationToken cancellationToken)
    {
        foreach (var key in keys)
        {
            memoryCache.Remove(key);
            await cache.RemoveAsync(key, cancellationToken);
        }
    }


    async Task IDistributedCacheRepository.RefreshCache(string key, CancellationToken cancellationToken) => await cache.RefreshAsync(key, cancellationToken);


    async Task IDistributedCacheRepository.SetEntityAsync<T>(string key, T entity, TimeSpan expiryTime, CancellationToken cancellationToken, bool setLocal)
    {
        if (entity == null)
            return;
        if (setLocal)
            memoryCache.Set(key, entity, TimeSpan.FromMinutes(10));
        var serializedData = entity.ToJson();
        var compressedData = await serializedData.Compress(cancellationToken);
        var dataStream = Encoding.UTF8.GetBytes(compressedData);
        await cache.SetAsync(key,
            dataStream,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = expiryTime
            },
            cancellationToken);
    }

    public async Task RemoveKeys(string keys, CancellationToken cancellationToken)
    {
        List<string> listKeys = [];
        using ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redConn);
        var servers = redis.GetServers();
        foreach (var server in servers)
        {
            RedisValue[] rcommand = [];
            var resp = await server.CommandGetKeysAsync(rcommand);
            foreach (var ritem in resp)
            {

            }
        }
    }

}