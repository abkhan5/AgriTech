using AgriTech.Domain.Sensors.Queries;
using AgriTech.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class MetaSearchService(ILogger<MetaSearchService> logger, IServiceScopeFactory serviceScopeFactory) : DomainSubscriptionWorker<SensorSearchQuery>(logger, serviceScopeFactory)
{
    public override DomainSubscriptionRequestDto GetSubscriptionRequest() =>
        new("Sensor", "temp");
}
