

using AgriTech.Dto;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisCacheExtension
{
    public static void AddRedisCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDistributedCacheRepository, RedisCacheRepository>();
        services.Configure<AzureRedisSettings>(configuration.GetSection(AzureRedisSettings.AzureRedisOptions));
        var redisSettings = new AzureRedisSettings();
        configuration.GetSection(AzureRedisSettings.AzureRedisOptions).Bind(redisSettings);
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
            options.InstanceName = $"{redisSettings.DatabaseName}".ToLower();
        });


        services.AddStackExchangeRedisOutputCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
            options.InstanceName = $"{redisSettings.DatabaseName}".ToLower();
        }
        );

        services.AddOutputCache(options =>
        {
            //options.AddBasePolicy(x =>
            //{
            //    x.Expire(TimeSpan.FromMinutes(5));
            //    x.SetCacheKeyPrefix("o~");
            //    x.Tag("base");
            //});
            options.AddPolicy(AgriTechConstants.OneDayCache, x =>
            {
                x.SetCacheKeyPrefix("oc~1d~");
                x.Expire(TimeSpan.FromDays(1));
            });
            options.AddPolicy(AgriTechConstants.OneHourCache, x =>
            {
                x.SetCacheKeyPrefix("oc~1h~");
                x.Expire(TimeSpan.FromHours(1));
            });
            options.AddPolicy(AgriTechConstants.FifteenMinCache, x =>
            {
                x.SetCacheKeyPrefix("oc~15m~");
                x.Expire(TimeSpan.FromMinutes(15));
            });
        });
    }


}
