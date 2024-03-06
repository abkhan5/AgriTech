using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriTech.Contracts.Options;

public record MetricSettings
{
    public const string SettingsName = "Metrics";

    public string OtelEndpoint { get; set; }
}


public sealed record AgriTechServiceSettings
{
    public const string AgriTechServiceOptions = "AgriTechServiceSettings";
    public string ServiceHost { get; set; }
    public string MessengerHubServiceHost { get; set; }
}