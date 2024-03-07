
namespace AgriTech.Datalayer.ElasticSearch;
internal class ElasticRepository<T>(IElasticClientFactory elasticClientFactory) : IElasticRepository<T> where T : BaseDto
{
    public readonly IElasticClientFactory elasticClientFactory = elasticClientFactory;
    private ElasticsearchClient?  GetClient()
    =>((Elastic8ClientFactory)elasticClientFactory).GetClient();
    

    public async Task Add(string index,T entity,CancellationToken cancellationToken) 
    {
        var client=GetClient();
        var response =await client.IndexAsync(entity, index,cancellationToken);      
    }

    public async Task<T?> Get(string index,T entity, CancellationToken cancellationToken)
    {
        var client = GetClient();

        var response = await client.
            SearchAsync<T>(s => s.Index(index).From(0).Size(10).Query(q => q.Term(t => entity.Id,index)),
            cancellationToken);
        return response.Documents.FirstOrDefault();
    }

    public async Task Delete(string  recordId,CancellationToken cancellationToken)
    {
        var client = GetClient();

        await client.DeleteAsync(recordId, 1,cancellationToken);
    }

    public async IAsyncEnumerable<T> GetAll(string index,CancellationToken cancellationToken)
    {
        var client = GetClient();
        var esResponse = await client.SearchAsync<T>(cancellationToken: cancellationToken);
        foreach (var item in esResponse.Documents)
            yield return item;
    }
}
