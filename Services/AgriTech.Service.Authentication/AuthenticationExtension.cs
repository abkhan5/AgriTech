using AgriTech.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgriTech.Service.Authentication;
public static class AuthenticationExtension
{

    public static void AddAgriTechEngAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IIdentityService, IdentityService>();
        
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromDays(1);
        });            
        services.ConfigureClaims(configuration);            
    }

    private static void ConfigureClaims(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireClaim("Admin"))
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }
}
