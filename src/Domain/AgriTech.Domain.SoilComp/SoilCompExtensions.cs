

namespace Microsoft.Extensions.DependencyInjection;

public static class SoilCompExtensions
{
    public static void AddEnvTemp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisCacheService(configuration);
        services.AddCosmosDbRepository(configuration);
        services.AddAzureMessagingServices(configuration);
    }
}