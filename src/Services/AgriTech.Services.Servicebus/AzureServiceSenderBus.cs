

namespace AgriTech.Services.Servicebus;

internal class AzureServiceSenderBus(
    ILogger<AzureServiceSenderBus> logger,
    AzureEntityManager entityManager,
    ServiceBusClient queueClient,
    IDistributedCacheRepository distributedCacheRepository)
    : IMessageSender
{
    private const string ScheduledMessageKey = "sc";
    private ServiceBusSender messageSender;

    public async Task SendMessage<T>(T message, string queueName, CancellationToken cancellationToken)
    {
        await entityManager.CreateQueue(queueName, cancellationToken);
        queueName = entityManager.GetLocalizedName(queueName);
        logger.LogInformation("Sending message to {queueName} ", queueName);
        var serializedData = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(serializedData);
        messageSender = queueClient.CreateSender(queueName);
        await messageSender.SendMessageAsync(serviceBusMessage, cancellationToken);
        logger.LogInformation("Sent message to {queueName} ", queueName);
    }

    public async Task SendMessage<T>(MessagePayload<T> message, string queueName, CancellationToken cancellationToken)
    {
        await entityManager.CreateQueue(queueName, cancellationToken);
        queueName = entityManager.GetLocalizedName(queueName);
        logger.LogInformation("Sending message to {queueName} ", queueName);
        var serializedData = JsonSerializer.Serialize(message.Payload);
        var serviceBusMessage = new ServiceBusMessage(serializedData)
        {
            MessageId = message.Id
        };

        messageSender = queueClient.CreateSender(queueName);

        if (message.ScheduledEnqueueTime.HasValue)
        {
            var messageId = message.Id;
            var existingRecords = await GetRecords(cancellationToken);
            if (existingRecords.Any(item => item.MessageId == messageId))
                await CancelMessage(messageId, cancellationToken);

            var longSequence = await messageSender.ScheduleMessageAsync(serviceBusMessage, message.ScheduledEnqueueTime.Value, cancellationToken);
            await CacheScheduleMessage(messageId, longSequence, cancellationToken);
        }
        else
            await messageSender.SendMessageAsync(serviceBusMessage, cancellationToken);

        logger.LogInformation("Sent message to {queueName} ", queueName);
    }

    private async Task<List<ScheduledMessageRecords>> GetRecords(CancellationToken cancellationToken) =>
        await distributedCacheRepository.GetAsync<List<ScheduledMessageRecords>>(ScheduledMessageKey, cancellationToken) ?? new List<ScheduledMessageRecords>();

    private async Task CacheScheduleMessage(string id, long sequenceNumber, CancellationToken cancellationToken)
    {
        var scheduledRecords = await GetRecords(cancellationToken);
        var currentScheduledRecord = scheduledRecords.FirstOrDefault(item => item.MessageId == id);
        scheduledRecords.Remove(currentScheduledRecord);
        scheduledRecords.Add(new ScheduledMessageRecords(id, sequenceNumber));
        await SaveScheduleMessage(scheduledRecords, cancellationToken);
    }

    private async Task SaveScheduleMessage(List<ScheduledMessageRecords> scheduledRecords, CancellationToken cancellationToken) =>
        await distributedCacheRepository.SetEntityAsync(ScheduledMessageKey, scheduledRecords, new TimeSpan(365, 0, 0, 0), cancellationToken);

    public async Task CancelMessage(string messageId, CancellationToken cancellationToken)
    {
        var scheduledRecords = await GetRecords(cancellationToken);
        var record = scheduledRecords.FirstOrDefault(item => item.MessageId == messageId);
        if (record == null)
            return;
        scheduledRecords.Remove(record);
        await SaveScheduleMessage(scheduledRecords, cancellationToken);
        await messageSender.CancelScheduledMessageAsync(record.SequenceNumber, cancellationToken);
    }


    public async Task SendMessage<T>(T message, CancellationToken cancellationToken) =>
        await SendMessage(message, message.GetType().Name, cancellationToken);

    public async Task SendMessages<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken)
    {
        await entityManager.CreateQueue(queueName, cancellationToken);
        queueName = entityManager.GetLocalizedName(queueName);
        logger.LogWarning("Sending message to {queueName} ", queueName);
        var serviceBusMessages = messages.Select(message => new ServiceBusMessage(JsonSerializer.Serialize(message)));
        messageSender = queueClient.CreateSender(queueName);
        await messageSender.SendMessagesAsync(serviceBusMessages, cancellationToken);
        logger.LogInformation("Sent message to {queueName} ", queueName);
    }

    public async Task Close(CancellationToken cancellationToken) =>
        await messageSender.CloseAsync(cancellationToken);


}
