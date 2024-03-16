using System.Text.Json.Serialization;

namespace AgriTech.Dto;

public abstract record BaseDto
{
    private string discriminator;

    [JsonPropertyName("id")] public string Id { get; set; }

    public string DeviceId { get; set; }

    public string UserId { get; set; }

    public string Discriminator
    {
        get => GetDiscriminator();
        set => discriminator = value;
    }

    public DateTime EventOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    [JsonPropertyName("_etag")]
    public string Etag { get; set; }
    protected virtual string GetDiscriminator()
    {
        if (string.IsNullOrEmpty(discriminator))
            discriminator = GetType().Name;
        return discriminator;
    }
}