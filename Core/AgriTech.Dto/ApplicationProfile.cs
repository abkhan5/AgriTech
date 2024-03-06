namespace AgriTech.Dto;

public record ApplicationProfile : BaseDto
{
    public string AppName { get; set; }
    public string BuildId { get; set; }
    public string MachineName { get; set; }
    public string ApplicationName { get; set; }
}