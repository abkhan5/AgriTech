
namespace AgriTech.Workers;
public abstract class MessageReceiverWorker : BackgroundService
{
    protected readonly ILogger logger;
    private readonly string messageName;
    private readonly IMessageReceiver messageReceiver;
    protected readonly IServiceScopeFactory serviceScopeFactory;

    public MessageReceiverWorker(ILogger<MessageReceiverWorker> logger, IMessageReceiver messageReceiver,
        IServiceScopeFactory serviceScopeFactory, string messageName)
    {
        this.logger = logger;
        this.messageReceiver = messageReceiver;
        this.serviceScopeFactory = serviceScopeFactory;
        this.messageName = messageName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    => await messageReceiver.OnMessageAsync(messageName, OnMessage, stoppingToken);



    protected string GetQueueName() => messageReceiver.QueueName;


    public abstract Task OnMessage(CancellationToken stoppingToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await messageReceiver.Stop(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}

public abstract class MessageReceiverWorker<T> : BackgroundService
{
    protected readonly ILogger logger;
    private readonly IMessageReceiver messageReceiver;
    protected readonly IServiceScopeFactory serviceScopeFactory;
    private readonly string messageName;

    public MessageReceiverWorker(ILogger<MessageReceiverWorker<T>> logger, IMessageReceiver messageReceiver,
        IServiceScopeFactory serviceScopeFactory, string messageName)
    {
        this.logger = logger;
        this.messageReceiver = messageReceiver;
        this.serviceScopeFactory = serviceScopeFactory;
        this.messageName = messageName;
    }

    protected string GetQueueName() => messageReceiver.QueueName;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"Starting {typeof(MessageReceiverWorker<T>).Name}");
        await messageReceiver.OnMessageAsync<T>(messageName, OnMessage, stoppingToken);
    }

    public abstract Task OnMessage(T message, CancellationToken stoppingToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await messageReceiver.Stop(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}