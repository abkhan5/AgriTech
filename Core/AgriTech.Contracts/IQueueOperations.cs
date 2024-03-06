namespace AgriTech.Contracts;

public interface IQueueOperations
{
    Task DeleteAll(CancellationToken cancellation);
}
