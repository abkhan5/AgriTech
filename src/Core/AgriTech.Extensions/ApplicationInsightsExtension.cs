using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AgriTech.Extensions;

internal static class ApplicationInsightsExtension
{
    private const string ApplicationInsightsConnectionString = "ApplicationInsightsConnectionString";

    public static IServiceCollection AddApplicationInsightsWebTelemetry(this IServiceCollection services, IConfiguration configuration)

    {
        var aiKey = configuration.GetValue<string>(ApplicationInsightsConnectionString);
        var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
        {
            ConnectionString = aiKey,
            EnableAdaptiveSampling = true,
            EnableQuickPulseMetricStream = true,
            EnableHeartbeat = true,
            EnablePerformanceCounterCollectionModule = true,
            AddAutoCollectedMetricExtractor = true,
            EnableEventCounterCollectionModule = true,
            EnableDependencyTrackingTelemetryModule = true,
            EnableAppServicesHeartbeatTelemetryModule = true,
            EnableActiveTelemetryConfigurationSetup = true,
            EnableDiagnosticsTelemetryModule = true,
            DependencyCollectionOptions =
                {
                    EnableLegacyCorrelationHeadersInjection = true
                },
            EnableAzureInstanceMetadataTelemetryModule = true,
            EnableDebugLogger = true,
            EnableAuthenticationTrackingJavaScript = false,
            EnableRequestTrackingTelemetryModule = true,
            RequestCollectionOptions =
                {
                    TrackExceptions = true,
                    InjectResponseHeaders  = true
                }
        };

        return services.AddApplicationInsightsTelemetry(aiOptions);
    }


    public static IServiceCollection AddApplicationInsightsWorkerTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var aiOptions = new Microsoft.ApplicationInsights.WorkerService.ApplicationInsightsServiceOptions
        {
            ConnectionString = configuration.GetValue<string>(ApplicationInsightsConnectionString),
            EnableAdaptiveSampling = true,
            EnableQuickPulseMetricStream = true,
            EnableHeartbeat = true,
            EnablePerformanceCounterCollectionModule = true,
            AddAutoCollectedMetricExtractor = false,
            EnableEventCounterCollectionModule = false,
            EnableDependencyTrackingTelemetryModule = false,
            EnableAppServicesHeartbeatTelemetryModule = true,
            EnableDiagnosticsTelemetryModule = true,
            EnableAzureInstanceMetadataTelemetryModule = true,
            EnableDebugLogger = false,
            DependencyCollectionOptions =
                {
                    EnableLegacyCorrelationHeadersInjection = false
                }
        };

        return services.AddApplicationInsightsTelemetryWorkerService(aiOptions);
    }
}