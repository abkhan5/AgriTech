using AgriTech.Extensions;

public class Startup : IAgriTechStartup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        
    }

    public void ConfigureApplication(IApplicationBuilder app)
    {
        app.ConfigureApp();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
