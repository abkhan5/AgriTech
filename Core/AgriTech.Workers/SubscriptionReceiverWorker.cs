namespace AgriTech.Workers;

public abstract class SubscriptionReceiverWorker<T> : BackgroundService
{
    protected readonly ILogger logger;
    protected readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IEventBusService eventBusSubscriptionsManager;
    private readonly SubscriptionApplicationProfile subscriptionApplicationProfile;
    private readonly ITableRepository<SubscriptionApplicationProfile> tableRepository;
    public SubscriptionReceiverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory, string topicName, EveryEngDomain domain, IEventBusService eventBusSubscriptionsManager)
        : this(logger, serviceScopeFactory, topicName, domain, false, eventBusSubscriptionsManager)
    {

    }
    public SubscriptionReceiverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory, string topicName, EveryEngDomain domain, bool toForward, IEventBusService eventBusSubscriptionsManager)
    {
        this.logger = logger;
        this.eventBusSubscriptionsManager = eventBusSubscriptionsManager;
        this.serviceScopeFactory = serviceScopeFactory;
        var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        tableRepository = serviceProvider.GetRequiredService<ITableRepository<SubscriptionApplicationProfile>>();
        var appProfile = serviceProvider.GetRequiredService<ApplicationProfile>();
        subscriptionApplicationProfile = new SubscriptionApplicationProfile
        {
            TopicName = topicName,
            SubscriptionName = domain.ToString(),
            CreatedOn = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            ApplicationName = appProfile.ApplicationName,
            AppName = appProfile.AppName,
            MachineName = appProfile.MachineName,
            TotalMessage = 0,
            State = JobStatusEnum.NotStarted,
            ToForwad = toForward
        };
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        logger.LogInformation("Starting {jobname}", typeof(MessageReceiverWorker<T>).Name);
        await eventBusSubscriptionsManager.Subscribe<T>(subscriptionApplicationProfile.TopicName, subscriptionApplicationProfile.SubscriptionName, subscriptionApplicationProfile.ToForwad, OnMessage, cancellationToken);
        await tableRepository.Upsert(subscriptionApplicationProfile, cancellationToken);
    }

    private async Task<bool> OnMessage(T message, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
            subscriptionApplicationProfile.TotalMessage++;
            await tableRepository.Upsert(subscriptionApplicationProfile, cancellationToken);
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
        subscriptionApplicationProfile.State = JobStatusEnum.Stopped;
        await tableRepository.Upsert(subscriptionApplicationProfile, cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        subscriptionApplicationProfile.State = JobStatusEnum.Running;
        await tableRepository.Upsert(subscriptionApplicationProfile, cancellationToken);
        await base.StartAsync(cancellationToken);
    }
}
