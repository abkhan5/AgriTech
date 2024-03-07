namespace AgriTech;

public static class UserSpecifications
{
    public static IAsyncEnumerable<TEntity> GetAllForUserId<TEntity>(this IRepository repository, string userId,
        CancellationToken cancellationToken) where TEntity : BaseDto =>
        repository.Get<TEntity>(r => r.DeviceId == userId, cancellationToken);


    public static async Task<bool> Exists<TEntity>(this IRepository repository, string id,
        CancellationToken cancellationToken) where TEntity : BaseDto
    {
        var entity = await repository.Get<TEntity>(id, cancellationToken);
        return entity != null;
    }

    public static async Task<bool> IsOwned<TEntity>(this IRepository repository, string id, string deviceId,
        CancellationToken cancellationToken) where TEntity : BaseDto
    {
        if (string.IsNullOrWhiteSpace(id))
            return true;
        var responseEntity = await repository.Get<TEntity>(id, cancellationToken);

        if (responseEntity == null)
            return true;

        return responseEntity.DeviceId == deviceId;
    }
}
