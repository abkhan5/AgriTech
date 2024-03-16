using Microsoft.Extensions.Configuration;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AgriTech.Dto;

namespace AgriTech.Extensions;

public static class AgritechAzureKeyVault
{
    
    public static void AddKeyVault(this IConfigurationBuilder appConfig, IConfigurationRoot config)
    {
        var keyVaultName = config[AgriTechConstants.AzureKeyVaultName];
        var managedId = config[AgriTechConstants.ManagedIdentityId];
        var tenantId = config[AgriTechConstants.TenantId];
        var useMachine = config.GetValue("useMachineName", false);

        TokenCredential userCred = useMachine ? new DefaultAzureCredential(new DefaultAzureCredentialOptions()
        {
            VisualStudioTenantId = tenantId,
            SharedTokenCacheTenantId = tenantId,
            ManagedIdentityClientId = managedId
        }) : new ManagedIdentityCredential(clientId: managedId);

        var options = new AzureKeyVaultConfigurationOptions
        {
            ReloadInterval = TimeSpan.FromMinutes(15),
            Manager = new KeyVaultSecretManager()
        };
        var url = $"https://{keyVaultName}.vault.azure.net";
        var client = new SecretClient(new Uri(url), userCred);
        appConfig.AddAzureKeyVault(client, options);
    }



}