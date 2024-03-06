namespace AgriTech.Workers;
public record DomainSubscriptionRequestDto(string TopicName, string Domain, bool DomainFilter = true, bool ToForward = true);
public abstract class DomainSubscriptionWorker<T>(ILogger logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger logger = logger;
    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        IEventBusService eventBusSubscriptionsManager = serviceProvider.GetRequiredService<IEventBusService>();
        var request = GetSubscriptionRequest();
        Dictionary<string, string> filters = [];
        var domain = request.Domain.ToString().ToLower();
        if (request.DomainFilter)
            filters.Add(domain, $"{domain}='{domain}'");
        await eventBusSubscriptionsManager.Subscribe<T>(request.TopicName, domain, request.ToForward, OnMessage, filters, cancellationToken);
    }

    private async Task<bool> OnMessage(T message, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;
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

    public abstract DomainSubscriptionRequestDto GetSubscriptionRequest();

}
