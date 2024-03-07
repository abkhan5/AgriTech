namespace AgriTech.Dto;

public record UserEventDto : BaseDto
{
    public string EventName { get; set; }
    public string Message { get; set; }
    public bool IsError { get; set; }
    public string Location { get; set; }
    public string RecordId { get; set; }
}
