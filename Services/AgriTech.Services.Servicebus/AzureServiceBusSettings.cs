namespace AgriTech.Services.Servicebus;

internal record AzureServiceBusSettings
{
    public const string AzureServiceBusSettingsName = "AzureServiceBusSettings";

    public string ConnectionString { get; set; }
    public bool UseMachineName { get; set; }
}