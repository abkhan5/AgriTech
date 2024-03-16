using AgriTech.Contracts.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTech.Extensions;

public static class MetricExtension
{
    private static Action<ResourceBuilder> GetActivityResource(string applcationName)
    {
        Action<ResourceBuilder> configureResource = r => r.AddService(serviceName: applcationName, serviceVersion: typeof(MetricExtension).Assembly.GetName().Version?.ToString() ?? "unknown",
    serviceInstanceId: Environment.MachineName);
        return configureResource;
    }
    public static void AddWebMetrics(this WebApplicationBuilder appBuilder, string applcationName)
    {
        appBuilder.Services.Configure<MetricSettings>(appBuilder.Configuration.GetSection(MetricSettings.SettingsName));

        appBuilder.Services.AddOpenTelemetry()
               .ConfigureResource(GetActivityResource(applcationName))
               .WithTracing(builder =>
               {
                   // Tracing

                   // Ensure the TracerProvider subscribes to any custom ActivitySources.
                   builder
                       .AddSource(applcationName)
                        .AddHttpClientInstrumentation()
                       .SetSampler(new AlwaysOnSampler())
                       .AddAspNetCoreInstrumentation();

                   // Use IConfiguration binding for AspNetCore instrumentation options.
                   //   appBuilder.Services.Configure<AspNetCoreInstrumentationOptions>(appBuilder.Configuration.GetSection("AspNetCoreInstrumentation"));


               })
    .WithMetrics(metrics =>
    {
        metrics
        .AddMeter(applcationName)
        .AddMeter("Microsoft.AspNetCore.Hosting")
        .AddMeter("Microsoft.AspNetCore.Routing")
        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
        .AddMeter("Microsoft.AspNetCore.Http.Connections")
        .AddMeter("System.Net.Http")
        .AddMeter("System.Net.NameResolution")
        // Metrics
        .AddView(intrumentations =>
        {
            if (intrumentations.Name == "http.server.request.duration")
                return new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = [0,
                        0.005,
                        0.01,
                        0.025,
                        0.05,
                        0.075,
                        0.1,
                        0.25,
                        0.5,
                        0.75,
                        1,
                        2.5,
                        5,
                        7.5,
                        10]
                };

            if (intrumentations.Name == "Requests")
                return new MetricStreamConfiguration
                {
                    Name = "HttpRequests",
                };

            return null;
        })
        .AddView(instrument =>
        {
            return instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                ? new Base2ExponentialBucketHistogramConfiguration()
                : null;
        })
            // Ensure the MeterProvider subscribes to any custom Meters.

            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter()
            .AddOtlpExporter(otlpOptions =>
            {
                var metricsSettings = new MetricSettings();
                appBuilder.Configuration.GetSection(MetricSettings.SettingsName).Bind(metricsSettings);
                // Use IConfiguration directly for Otlp exporter endpoint option.
                otlpOptions.Endpoint = new Uri(metricsSettings?.OtelEndpoint ?? "http://localhost:4317");
                otlpOptions.ExportProcessorType = ExportProcessorType.Simple;
            });
    });

    }

    public static void AddConsoleMetrics(this IServiceCollection services, IConfiguration configuration, string applcationName)
    {
        services.Configure<MetricSettings>(configuration.GetSection(MetricSettings.SettingsName));
        services.AddOpenTelemetry()
            .ConfigureResource(GetActivityResource(applcationName))
            .WithTracing(builder =>
            {
                builder
                .AddSource(applcationName)
                .SetSampler(new AlwaysOnSampler());
            })
            .WithMetrics(metrics =>
            {
                metrics
             .AddMeter(applcationName)
             .AddMeter("Microsoft.AspNetCore.Hosting")
             .AddMeter("Microsoft.AspNetCore.Routing")
             .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
             .AddMeter("Microsoft.AspNetCore.Http.Connections")
             .AddMeter("System.Net.Http")
             .AddMeter("System.Net.NameResolution")
             .AddHttpClientInstrumentation()
             .AddRuntimeInstrumentation()
             .AddPrometheusExporter()
             .AddOtlpExporter(otlpOptions =>
             {
                 var metricsSettings = new MetricSettings();
                 configuration.GetSection(MetricSettings.SettingsName).Bind(metricsSettings);
                 // Use IConfiguration directly for Otlp exporter endpoint option.
                 otlpOptions.Endpoint = new Uri(metricsSettings?.OtelEndpoint ?? "http://localhost:4317");
             }); ;
            });
    }
}
