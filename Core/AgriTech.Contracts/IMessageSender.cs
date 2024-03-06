namespace AgriTech.Contracts;

public interface IMessageSender
{
    Task SendMessage<T>(T message, CancellationToken cancellationToken);

    Task SendMessage<T>(T message, string queueName, CancellationToken cancellationToken);
    Task SendMessage<T>(MessagePayload<T> message, string queueName, CancellationToken cancellationToken);

    Task SendMessages<T>(IEnumerable<T> messages, string queueName, CancellationToken cancellationToken);

    Task CancelMessage(string messageId, CancellationToken cancellationToken);
    Task Close(CancellationToken cancellationToken);
}
