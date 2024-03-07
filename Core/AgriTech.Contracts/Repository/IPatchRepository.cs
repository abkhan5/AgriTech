namespace AgriTech;

public interface IPatchRepository
{
    Task DeletePatch<TEntity>(string id, IEnumerable<string> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;

    Task Patch<TEntity>(TEntity entity, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Patch<TEntity>(TEntity entity, (string, object) patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task PatchFor<TEntity>(TEntity entity, string filter, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task PatchFor<TEntity>(TEntity entity, string filter, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;





    Task Patch<TEntity>(string id, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Patch<TEntity>(string id, string partitionKey, (string, object) patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Patch<TEntity>(string id, (string, object) patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task PatchFor<TEntity>(string id, string filter, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task PatchFor<TEntity>(string id, string filter, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto;
    Task Increment<TEntity>(string id, (string, double) patchField, CancellationToken cancellationToken) where TEntity : BaseDto;
}