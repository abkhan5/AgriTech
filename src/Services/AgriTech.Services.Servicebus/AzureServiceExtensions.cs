namespace Microsoft.Extensions.DependencyInjection;

public static class AzureServiceExtensions
{
    public static void AddAzureMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<AzureEntityManager>();
        services.Configure<AzureServiceBusSettings>(
            configuration.GetSection(AzureServiceBusSettings.AzureServiceBusSettingsName));
        //services.AddTransient<AzureQueueReceiver>();
        services.AddTransient<AzureMessageProcessor>();
        services.AddTransient<AzureServiceSenderBus>();
        services.AddTransient<IMessageSender, AzureServiceSenderBus>();
        services.AddTransient<IMessageReceiver, AzureMessageProcessor>();
        services.AddTransient<IQueueOperations, AzureEntityManager>();
        services.AddTransient<IEventBusService, AzureEventBusSubscriptionsManager>();
        services.AddAzureClients(builder =>
        {
            builder.AddClient<ServiceBusClient, ServiceBusClientOptions>((options, provider) =>
            {
                var appOptions = provider.GetService<IOptions<AzureServiceBusSettings>>();
                return new ServiceBusClient(appOptions.Value.ConnectionString);
            });
        });
        services.AddTokenCredential();
    }

    private static void AddTokenCredential(this IServiceCollection services)
        =>services.AddSingleton<TokenCredential>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var logger = sp.GetRequiredService<ILogger<TokenCredential>>();
                var managedId = config[AgriTechConstants.ManagedIdentityId];
                logger.LogCritical("Managed Identity {ManagedIdentity}", managedId);
                return new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = managedId
                });
            });
}
