namespace AgriTech;

public record MessagePayload<T>
{
    public T Payload { get; set; }

    public string Id { get; set; }
    public DateTimeOffset? ScheduledEnqueueTime { get; set; }
}