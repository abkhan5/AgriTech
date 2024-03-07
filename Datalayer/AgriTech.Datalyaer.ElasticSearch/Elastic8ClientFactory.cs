
namespace AgriTech.Datalayer.ElasticSearch;

public class Elastic8ClientFactory(IOptions<ElasticSearchSettings> options) : IElasticClientFactory
{
    private ElasticsearchClient? elasticClient;
    private readonly IOptions<ElasticSearchSettings> options = options;

    public ElasticsearchClient GetClient()
    {
        elasticClient ??= new ElasticsearchClient(new Uri(options.Value.BaseUrl));
        //CreateIndex(client, index);
        return elasticClient;
    }
}
