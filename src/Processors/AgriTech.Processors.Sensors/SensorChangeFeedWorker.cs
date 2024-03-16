using AgriTech.Domain.Sensors.Command;
using AgriTech.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

public class SensorChangeFeedWorker(ILogger<SensorChangeFeedWorker> logger, IDataBatchProcessor dataBatchProcessor, IServiceScopeFactory serviceScopeFactory) : CosmosChangeFeedWorker(logger, dataBatchProcessor, "sensors", leaseContainerName, serviceName)
{
    private const string leaseContainerName = "sensorslease";
    private const string serviceName = "sensorchangefeed";

    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    protected override async Task HandleChangesAsync(IReadOnlyCollection<JObject> changes, CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new SensorChangeFeedCommand(changes), cancellationToken);
    }
}
