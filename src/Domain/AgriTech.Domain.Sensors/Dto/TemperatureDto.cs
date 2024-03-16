
namespace AgriTech.Domain.Sensors.Dto;

public record TemperatureDto:BaseDto
{
    public double Temperature { get; set; }
    public string Unit { get; set; } = "Fahrenheit";
    public string EventTimestamp { get; set; } = DateTime.UtcNow.ToString("s");
    public string ReceivedTimestamp { get; set; } = DateTime.UtcNow.ToString("s");
}


public record Reading
{
    public string? eventTimestamp { get; set; }
    public double temperature { get; set; }

}
public record TemperatureSummaryEvent:BaseDto
{  
    public int NumberOfReadings { get; set; }
    public double AvgTemperature { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }        
}