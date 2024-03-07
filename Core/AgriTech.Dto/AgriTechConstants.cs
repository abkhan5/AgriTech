using System.ComponentModel.DataAnnotations;

namespace AgriTech.Dto;

public class AgriTechConstants
{
    public const string EntityId = "id";
    public const string ManagedIdentityId = "ManagedIdentityId";
    public const string TenantId = "TenantId";
    public const string AzureKeyVaultName = "AzureKeyVaultName";
    public const string OneDayCache = "OneDayCache";
    public const string OneHourCache = "OneHourCache";
    public const string FifteenMinCache = "FifteenMinCache";
    public const string EntityDiscriminator = "discriminator";
}

public abstract record BaseEntity<T> : BaseEntity
{
    [Key] public virtual T Id { get; set; }
}

public abstract record BaseEntity
{
}