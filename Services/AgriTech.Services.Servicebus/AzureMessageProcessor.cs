


namespace AgriTech.Services.Servicebus;

internal class AzureMessageProcessor(
    ILogger<AzureMessageProcessor> logger,
    AzureEntityManager azureEntityManager,
    ServiceBusClient queueClient,
    IMeterFactory meterFactory,
    ApplicationProfile applicationProfile) : IMessageReceiver
{
    private readonly AzureEntityManager azureEntityManager = azureEntityManager;
    private readonly ILogger<AzureMessageProcessor> logger = logger;
    private readonly ServiceBusClient queueClient = queueClient;
    private ServiceBusProcessor processor;
    private readonly IMeterFactory meterFactory = meterFactory;
    ActivitySource source = new(applicationProfile.ApplicationName);

    public string QueueName { get; private set; }

    public async Task Stop(CancellationToken token)
    {
        //   await processor.StopProcessingAsync(token);
        if (processor != null)
            await processor.CloseAsync(token);
    }


    public async Task OnMessageAsync(string queueName, Func<CancellationToken, Task> callBack, CancellationToken token)
    {
        await azureEntityManager.CreateQueue(queueName, token);
        QueueName = queueName = azureEntityManager.GetLocalizedName(queueName);
        logger.LogInformation("Receiver set for {queueName}", queueName);
        processor = queueClient.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxConcurrentCalls = 2,
            PrefetchCount = 2,
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        });
        processor.ProcessMessageAsync += async responseItem => { await callBack(token); };
        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        await processor.StartProcessingAsync(token);
    }

    public async Task OnMessageAsync<T>(string queueName, Func<T, CancellationToken, Task> callBack,
        CancellationToken cancellationToken)
    {
        await azureEntityManager.CreateQueue(queueName, cancellationToken);
        QueueName = queueName = azureEntityManager.GetLocalizedName(queueName);
        logger.LogInformation("Receiver set for {queueName}", queueName);
        processor = queueClient.CreateProcessor(queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxConcurrentCalls = 1,
            PrefetchCount = 2,
            ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
        });
        processor.ProcessMessageAsync += async responseItem =>
        {
            var body = responseItem.Message.Body;
            if (body == null)
                return;
            using var activity = source.StartActivity($"{QueueName} processing");
            var payload = body.ToObjectFromJson<T>();
            await callBack(payload, responseItem.CancellationToken);
        };
        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        await processor.StartProcessingAsync(cancellationToken);
    }


    private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        logger.LogError(arg.Exception.ToString());
        logger.LogError(arg.Exception.InnerException?.ToString());
        return Task.CompletedTask;
    }
}