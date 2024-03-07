namespace AgriTech;

public record AzureStorageSASResult
{
    public Uri SASUri { get; set; }
    public DateTimeOffset SASExpire { get; set; }
    public string IpAddress { get; set; }
}

public record BlobStoreItem(string Content, Dictionary<string, string> Metadata, string FileName, string ContentType);
public record BlobStoreItemStream(Stream DataStream, IDictionary<string, string> Metadata, string FileName, string ContentType);
public record BlobStorageItem
{
    public string FolderName { get; set; }
    private string containerName;
    private string fileName;

    public string ContainerName
    {
        get => containerName;
        set => containerName = value?.ToLower();
    }

    public string FileName
    {
        get => fileName;
        set => fileName = value?.ToLower();
    }


    public string ContentType { get; set; }
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    public Stream DataStream { get; set; }
    public bool ToEncryptContent { get; set; }
    public static string ToAzureKeyString(string str)
    {
        var sb = new StringBuilder();
        foreach (var c in str
                     .Where(c => c != '/'
                                 //    && c != '\\'
                                 && c != '#'
                                 && c != '/'
                                 && c != '?'
                                 && c != '@'
                                 && !char.IsControl(c)))
            sb.Append(c);
        return sb.ToString();
    }
}