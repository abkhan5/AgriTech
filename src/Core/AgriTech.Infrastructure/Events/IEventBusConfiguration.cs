namespace AgriTech.Infrastructure.Events;

public interface IEventBusConfiguration
{
    string ServiceBusConfiguration { get; }
}