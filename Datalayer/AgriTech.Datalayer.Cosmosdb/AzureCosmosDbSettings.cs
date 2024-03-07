namespace AgriTech;

public sealed record AzureCosmosDbSettings
{
    public const string CosmosDbOptions = "CosmosDbSettings";
    public string Account { get; set; }
    public string Key { get; set; }
    public string DatabaseName { get; set; }
    public string ContainerName { get; set; }
    public string ContainerNames { get; set; }

    public string GetConnectionString()
    {
        return $"{Account};AccountKey={Key};";
    }

    public IEnumerable<string> GetContainerNames()
    {
        return ContainerNames?.Split(",");
    }
}
