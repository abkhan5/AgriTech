
namespace Microsoft.Extensions.DependencyInjection;
public static class CosmosDbRepositoryExtensions
{
    public static void AddCosmosDbRepository(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<AzureCosmosDbSettings>(configuration.GetSection(AzureCosmosDbSettings.CosmosDbOptions));
        service.AddTransient(typeof(ISearchService<>), typeof(SearchCosmosDbSqlService<>));
        service.AddTransient<IRepository, CosmosDbSqlRepository>();
        service.AddTransient<IDataBatchProcessor, DataBatchProcessor>();
        service.TryAddSingleton(sp =>
        {
            var resp = new CosmosClientFactory(sp.GetRequiredService<ApplicationProfile>());
            var cosmosDbOptions = sp.GetRequiredService<IOptions<AzureCosmosDbSettings>>();
            resp.CreateClientAsync(true, cosmosDbOptions, CancellationToken.None).GetAwaiter().GetResult();
            return resp;
        });
    }
}

