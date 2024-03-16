using AgriTech.Dto;

namespace AgriTech.Workers;

public abstract class SubscriptionReceiverWorker<T> : BackgroundService
{
    protected readonly ILogger logger;
    protected readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IEventBusService eventBusSubscriptionsManager;
    private readonly SubscriptionApplicationProfile subscriptionApplicationProfile;
    public SubscriptionReceiverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory, string topicName, string domain, IEventBusService eventBusSubscriptionsManager)
        : this(logger, serviceScopeFactory, topicName, domain, false, eventBusSubscriptionsManager)
    {

    }
    public SubscriptionReceiverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory, string topicName, string domain, bool toForward, IEventBusService eventBusSubscriptionsManager)
    {
        this.logger = logger;
        this.eventBusSubscriptionsManager = eventBusSubscriptionsManager;
        this.serviceScopeFactory = serviceScopeFactory;
        var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        var appProfile = serviceProvider.GetRequiredService<ApplicationProfile>();
        subscriptionApplicationProfile = new SubscriptionApplicationProfile
        {
            TopicName = topicName,
            SubscriptionName = domain.ToString(),
            EventOn = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            ApplicationName = appProfile.ApplicationName,
            AppName = appProfile.AppName,
            MachineName = appProfile.MachineName,
            TotalMessage = 0,
            ToForwad = toForward
        };
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        logger.LogInformation("Starting {jobname}", typeof(MessageReceiverWorker<T>).Name);
        await eventBusSubscriptionsManager.Subscribe<T>(subscriptionApplicationProfile.TopicName, subscriptionApplicationProfile.SubscriptionName, subscriptionApplicationProfile.ToForwad, OnMessage, cancellationToken);
    }

    private async Task<bool> OnMessage(T message, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            subscriptionApplicationProfile.TotalMessage++;
            var mediatr = serviceProvider.GetRequiredService<IMediator>();
            await mediatr.Send(message, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Sub failed for {message}", message);
            return false;
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }
}
