namespace AgriTech.Domain.Sensors.Command;

public record SummarizeTemperatureCommand(TemperatureDto Temperature):ICommand;
