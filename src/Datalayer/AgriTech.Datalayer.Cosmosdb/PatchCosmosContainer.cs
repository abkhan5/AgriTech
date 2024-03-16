namespace AgriTech;
public partial class CosmosDbSqlRepository : CosmosBaseRepository, IRepository
{

    public Task Patch<TEntity>(TEntity entity, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto
        => Patch<TEntity>(entity.Id, patchFields, cancellationToken);
    public Task Patch<TEntity>(TEntity entity, (string, object) patchFields, CancellationToken cancellationToken) where TEntity : BaseDto
    => Patch<TEntity>(entity.Id, patchFields, cancellationToken);
    public Task PatchFor<TEntity>(TEntity entity, string filter, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto
     => PatchFor<TEntity>(entity.Id, filter, patchField, cancellationToken);

    public Task PatchFor<TEntity>(TEntity entity, string filter, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto
         => PatchFor<TEntity>(entity.Id, filter, patchFields, cancellationToken);



    public Task Increment<TEntity>(string id, (string, double) patchField, CancellationToken cancellationToken) where TEntity : BaseDto =>
        Patch(id, new List<PatchOperation>(1) { PatchOperation.Increment($"/{patchField.Item1}", patchField.Item2) }, cancellationToken);

    public Task PatchFor<TEntity>(string id, string filter, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto =>
        PatchFor<TEntity>(id, filter, new List<(string, object)>(1) { patchField }, cancellationToken);

    public async Task PatchFor<TEntity>(string filter, string id, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto =>
        await Patch(id, patchFields.Select(item => PatchOperation.Set($"/{item.Item1}", item.Item2)), cancellationToken, new PatchItemRequestOptions
        {
            FilterPredicate = filter,
        });

    public Task Patch<TEntity>(string id, string partitionKey, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto
       => Patch<TEntity>(id, partitionKey, new List<(string, object)>(1) { patchField }, cancellationToken);

    public Task Patch<TEntity>(string id, (string, object) patchField, CancellationToken cancellationToken) where TEntity : BaseDto =>
        Patch<TEntity>(id, new List<(string, object)>(1) { patchField }, cancellationToken);

    public async Task Patch<TEntity>(string id, IReadOnlyList<(string, object)> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto =>
        await Patch(id, patchFields.Select(item => PatchOperation.Set($"/{item.Item1}", item.Item2)), cancellationToken);


    public async Task DeletePatch<TEntity>(string id, IEnumerable<string> patchFields, CancellationToken cancellationToken) where TEntity : BaseDto =>
        await Patch(id, patchFields.Select(item => PatchOperation.Remove($"/{item}")), cancellationToken);

    private async Task Patch(string id, IEnumerable<PatchOperation> ops, CancellationToken cancellationToken, PatchItemRequestOptions patchItemRequest = null)
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        await container.PatchItemStreamAsync(id, new PartitionKey(id), ops.ToList(), patchItemRequest ?? new PatchItemRequestOptions
        {
            EnableContentResponseOnWrite = false
        }, cancellationToken: cancellationToken);
    }
    private async Task Patch<TEntity>(string id, string partitionKey, List<(string, object)> ops, CancellationToken cancellationToken, PatchItemRequestOptions patchItemRequest = null) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        await container.PatchItemAsync<TEntity>(id, new PartitionKey(partitionKey), ops.Select(item => PatchOperation.Set($"/{item.Item1}", item.Item2)).ToList(), patchItemRequest ?? new PatchItemRequestOptions
        {
            EnableContentResponseOnWrite = false
        }, cancellationToken: cancellationToken);
    }

    private async Task Patch<TEntity>(string id, string partitionKey, IEnumerable<PatchOperation> ops, CancellationToken cancellationToken, PatchItemRequestOptions patchItemRequest = null) where TEntity : BaseDto
    {
        var container = await GetCosmosContainer(false, cancellationToken);
        await container.PatchItemAsync<TEntity>(id, new PartitionKey(partitionKey), ops.ToList(), patchItemRequest ?? new PatchItemRequestOptions
        {
            EnableContentResponseOnWrite = false
        }, cancellationToken: cancellationToken);
    }
}