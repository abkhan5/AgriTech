using AgriTech.Domain.Sensors.CommandHandler;
using AgriTech.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup : IAgriTechStartup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<SensorChangeFeedWorker>();
        services.AddHostedService<MetaSearchService>();
    }

    
}
