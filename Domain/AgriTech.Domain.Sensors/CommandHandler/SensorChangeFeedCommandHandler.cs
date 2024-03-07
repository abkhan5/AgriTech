


namespace AgriTech.Domain.Sensors.CommandHandler;
public sealed class SensorChangeFeedCommandHandler(IRepository repository, IEventBusService eventMessger, IDistributedCacheRepository distributedCacheRepository, IMediator mediator) : ICommandHandler<SensorChangeFeedCommand>
{
    private readonly IDistributedCacheRepository distributedCacheRepository = distributedCacheRepository;
    private readonly IRepository repository = repository;
    private readonly IEventBusService eventMessger = eventMessger;
    private readonly IMediator mediator = mediator;
    public async Task Handle(SensorChangeFeedCommand request, CancellationToken cancellationToken)
    {
        foreach (var changeItem in request.Changes)
        {
            var discriminatorValue = changeItem[AgriTechConstants.EntityDiscriminator].ToString();
            switch (discriminatorValue)
            {

                case nameof(TemperatureDto):
                    var tempDto = JsonConvert.DeserializeObject<TemperatureDto>(changeItem.ToString());
                    await mediator.Send(new SummarizeTemperatureCommand(tempDto), cancellationToken);
                    break;
            }
        }
    }
}
