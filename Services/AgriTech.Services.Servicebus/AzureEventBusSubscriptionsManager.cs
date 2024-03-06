namespace AgriTech.Services.Servicebus;

internal class AzureEventBusSubscriptionsManager(
    ILogger<AzureEventBusSubscriptionsManager> logger,
    AzureEntityManager azureEntityManager,
    ServiceBusClient client)
    : IEventBusService
{
    private readonly ILogger logger = logger;
    private ServiceBusProcessor processor;

    public async Task Subscribe<T>(string topicName, string subscriptionName, bool toForward, Func<T, CancellationToken, Task<bool>> callBack, IDictionary<string, string> traits, CancellationToken cancellationToken)
    {
        await azureEntityManager.CreateTopic(topicName, cancellationToken);
        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxConcurrentCalls = 10,
            PrefetchCount = 2,
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        };
        if (toForward)
        {
            string forwardingQueueName = azureEntityManager.GetLocalizedName($"{topicName}-{subscriptionName}");
            await azureEntityManager.CreateQueue(forwardingQueueName, cancellationToken);
            await azureEntityManager.CreateTopicSubscription(topicName, subscriptionName, forwardingQueueName, traits, cancellationToken);
            processor = client.CreateProcessor(forwardingQueueName, options);
        }
        else
        {
            var subName = azureEntityManager.GetLocalizedName(subscriptionName);
            await azureEntityManager.CreateTopicSubscription(topicName, subName, null, traits, cancellationToken);
            processor = client.CreateProcessor(topicName, subName, options);
        }

        processor.ProcessMessageAsync += async responseItem =>
        {
            var body = responseItem.Message.Body;
            if (body == null)
                return;
            var payload = body.ToObjectFromJson<T>();
            await callBack(payload, responseItem.CancellationToken);
        };
        processor.ProcessErrorAsync += ErrorHandler;
        await processor.StartProcessingAsync(cancellationToken);
    }

    public Task Subscribe<T>(string topicName, string subscriptionName, bool toForward, Func<T, CancellationToken, Task<bool>> callBack, CancellationToken cancellationToken) =>
        Subscribe(topicName, subscriptionName, toForward, callBack, new Dictionary<string, string>(), cancellationToken);

    public Task Publish<T>(T domainEvent, string topicName, CancellationToken token)
    => client.CreateSender(topicName).SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(domainEvent)), token);
    
    public async Task Publish<T>(T domainEvent, string topicName, IDictionary<string, string> traits, CancellationToken cancellationToken)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(domainEvent));
        foreach (var item in traits)
            message.ApplicationProperties.Add(item.Key, item.Value);

        var sender = client.CreateSender(topicName);
        await sender.SendMessageAsync(message, cancellationToken);
        await sender.CloseAsync(cancellationToken);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        logger.LogCritical(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message,
            context);
        return Task.CompletedTask;
    }

}