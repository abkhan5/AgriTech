namespace AgriTech.Dto;

public record ApplicationProfile : BaseDto
{
    public string AppName { get; set; }
    public string BuildId { get; set; }
    public string MachineName { get; set; }
    public string ApplicationName { get; set; }
}
public record SubscriptionApplicationProfile : BaseDto
{
    public string AppName { get; set; }
    public string MachineName { get; set; }
    public string ApplicationName { get; set; }
    public string SubscriptionName { get; set; }
    public string TopicName { get; set; }
    public int TotalMessage { get; set; }
    public bool ToForwad { get; set; }
}
