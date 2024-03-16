



using System.Threading;

namespace AgriTech.Domain.Sensors.CommandHandler;

public sealed class TempertatureCommandHandler(IRepository repository) : 
    ICommandHandler<SummarizeTemperatureCommand>
{
    private readonly IRepository repository = repository;
    

    public async Task Handle(SummarizeTemperatureCommand request, CancellationToken cancellationToken)
    {
        var temperature = request.Temperature;
        var minSummaryId = temperature.EventOn.ToString("yyyy-MM-dd'T'HH:mm'");
        var evenDate = temperature.EventOn;
        await CheckAndUpdateSummary(minSummaryId, 
            new DateTime(evenDate.Year, evenDate.Month, evenDate.Day, evenDate.Hour, evenDate.Minute,0),
            new DateTime(evenDate.Year, evenDate.Month, evenDate.Day, evenDate.Hour, evenDate.Minute, 60),
            temperature,cancellationToken);

        var hourSummaryId = temperature.EventOn.ToString("yyyy-MM-dd'T'HH'");
        await CheckAndUpdateSummary(minSummaryId,
            new DateTime(evenDate.Year, evenDate.Month, evenDate.Day, evenDate.Hour, 0, 0),
            new DateTime(evenDate.Year, evenDate.Month, evenDate.Day, evenDate.Hour, 60, 60),
            temperature, cancellationToken);

        var daySummaryId = temperature.EventOn.ToString("yyyy-MM-dd");
        await CheckAndUpdateSummary(minSummaryId,evenDate.Date,evenDate.Date.AddSeconds(-1),temperature, cancellationToken);


    }


    private async Task CheckAndUpdateSummary(string summaryId,DateTime offset,DateTime limit,  TemperatureDto temperature,CancellationToken cancellationToken)
    {
        var summary = await repository.Get<TemperatureSummaryEvent>(summaryId, cancellationToken);

        if (summary == null) 
        {
            summary = new()
            {
                DeviceId = temperature.DeviceId,
                UserId = temperature.UserId,
                Id = summaryId,
                AvgTemperature = temperature.Temperature,
                NumberOfReadings = 1,
                MaxTemperature = temperature.Temperature,
                MinTemperature = temperature.Temperature,
                EventOn=DateTime.UtcNow,
                UpdatedOn=DateTime.UtcNow,
            };
            await repository.Create(summary,cancellationToken);
        }
        else
        {
            if (summary.MaxTemperature < temperature.Temperature)
                summary.MaxTemperature = temperature.Temperature;

            if (summary.MinTemperature > temperature.Temperature)
                summary.MinTemperature = temperature.Temperature;
            
            summary.NumberOfReadings++;
            var averageTemperatureQuery= $"SELECT Value Avg(r.Temperature) FROM r Where r.Discriminator='{nameof(TemperatureDto)}' and r.EventOn>{offset} and r.EventOn<{limit}";

            summary.AvgTemperature =await repository.GetSum(averageTemperatureQuery,cancellationToken);
           await repository.Update(summary, cancellationToken);
        }
    }
}
