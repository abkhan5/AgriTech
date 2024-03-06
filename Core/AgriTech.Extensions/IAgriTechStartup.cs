using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgriTech.Extensions;

public interface IAgriTechStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    public void ConfigureApplication(IApplicationBuilder app)
    {
        app.ConfigureApp();
    }
}
