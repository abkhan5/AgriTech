namespace AgriTech;

public interface IQueueOperations
{
    Task DeleteAll(CancellationToken cancellation);
}
