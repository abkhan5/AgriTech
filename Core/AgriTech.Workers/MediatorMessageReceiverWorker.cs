namespace AgriTech.Workers;

public abstract class MediatorMessageReceiverWorker<TCommand> : BackgroundService
{
    protected readonly ILogger logger;
    private readonly IMessageReceiver messageReceiver;
    protected readonly IServiceScopeFactory serviceScopeFactory;
    private readonly string messageName;

    public MediatorMessageReceiverWorker(ILogger<MediatorMessageReceiverWorker<TCommand>> logger,
        IMessageReceiver messageReceiver, IServiceScopeFactory serviceScopeFactory, string messageName)
    {
        this.logger = logger;
        this.messageReceiver = messageReceiver;
        this.serviceScopeFactory = serviceScopeFactory;
        this.messageName = messageName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Starting {typeof(MediatorMessageReceiverWorker<TCommand>).Name}");
        await messageReceiver.OnMessageAsync<TCommand>(messageName, OnMessage, stoppingToken);
    }

    public virtual async Task OnMessage(TCommand command, CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var mediatr = serviceProvider.GetRequiredService<IMediator>();
        await mediatr.Send(command, stoppingToken);
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await messageReceiver.Stop(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}