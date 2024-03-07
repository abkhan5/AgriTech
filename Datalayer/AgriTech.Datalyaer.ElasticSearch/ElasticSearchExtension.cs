namespace Microsoft.Extensions.DependencyInjection;

public static class ElasticSearchExtension
{
    public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticSearchSettings>(configuration.GetSection(ElasticSearchSettings.ElasticSearchOptions));
        services.AddSingleton<IElasticClientFactory,Elastic8ClientFactory>();        
        services.AddTransient(typeof(IElasticRepository<>), typeof(ElasticRepository<>));
    }
}