namespace AgriTech.Datalayer.ElasticSearch;

public record ElasticSearchSettings
{
    public const string ElasticSearchOptions = "ElasticSearchSettings";
    public string BaseUrl { get; set; }
    public string DefaultIndex { get; set; }
    public string ThumbPrint { get; set; }
}
