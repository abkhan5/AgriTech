using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using AgriTech.Service.Authentication;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AgriTech.Dto;
using AgriTech.Contracts.Options;
using AgriTech.Extensions;
using Google.Protobuf.WellKnownTypes;

namespace AgriTech;
public static class ProgramExtensions
{
    internal static string ApplicationName;
    public static void CreateSerilogLogger(string appName)
    {
        ApplicationName = appName;
    }

    public static async Task RunConsoleHost<T>(this IHostBuilder builder) where T : IAgriTechStartup, new()
    {
        builder.ConfigureAppConfiguration((hostingContext, config) => GetConfiguration(hostingContext.HostingEnvironment, config));
        builder.ConfigureServices((context, services) =>
        {
            var config = context.Configuration;
            var agritechStartup = new T();
            services.AddAgriTechEngAuthentication(config);
            agritechStartup.ConfigureServices(services, config);
            services.AddApplicationProfile();
            services.AddApplicationInsightsWorkerTelemetry(config);
            services.AddConsoleMetrics(config, ApplicationName);
        });
        var app = builder.Build();
        //app.MapObservability();
        await app.RunHost();
    }

    public static void RegisterAssemblies(this IServiceCollection services, Type[] types)
    {
        var appTypes = types.Distinct().ToArray();
        services.AddAutoMapper(appTypes);
        var assemblies = appTypes.Select(t => t.Assembly).Distinct().ToArray();
        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
    }

    private static async Task RunHost<T>(this T app) where T : IHost
    {
        await app.RunAsync();
    }

    public static async Task RunAgriTechHost<T>(this WebApplicationBuilder builder) where T : IAgriTechStartup, new()
    {
        Log.Logger.Information($"Current Env is {builder.Environment.EnvironmentName}");
        builder.Host.ConfigureagritechHost();
        var agritechStartup = new T();
        builder.AddWebMetrics(ApplicationName);
        builder.Services.AddAgriTechEngAuthentication(builder.Configuration);
        builder.Services.AddApplicationInsightsWebTelemetry(builder.Configuration);        
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        agritechStartup.ConfigureServices(builder.Services, builder.Configuration);
        builder.Services.AddApiControllers(builder.Configuration);
        builder.Services.AddApplicationProfile();

        var app = builder.Build();
        //if (app.Environment.IsDevelopment())
        app.UseOutputCache();
        agritechStartup.ConfigureApplication(app);
        app.MapControllers();
        app.MapPrometheusScrapingEndpoint().AllowAnonymous();

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.MapRazorComponents().AddInteractiveServerRenderMode();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        await app.RunHost();
    }
    private static void AddApplicationProfile(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var appProfile = sp.GetRequiredService<IOptions<ApplicationProfileSetting>>().Value;
            return new ApplicationProfile
            {
                Id = ApplicationName,
                AppName = ApplicationName,
                UserId = Environment.MachineName,
                MachineName = Environment.MachineName,
                CreatedOn = DateTime.UtcNow,
                BuildId = appProfile.BuildNumber,
                ApplicationName = ApplicationName
            };
        });
    }

   

    public static IHostBuilder ConfigureagritechHost(this IHostBuilder host) =>
        host
        .UseSerilog()
        .ConfigureAppConfiguration((hostingContext, config) => GetConfiguration(hostingContext.HostingEnvironment, config));


    private static void GetConfiguration(IHostEnvironment hostingContext, IConfigurationBuilder appConfig)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("agritechsetting.json", false, true)
            .AddEnvironmentVariables();
        var config = builder.Build();
        appConfig.AddConfiguration(config);
 
        var useVault = config.GetValue("UseVault", false);
        if (useVault)
            appConfig.AddKeyVault(config);
    }
}
