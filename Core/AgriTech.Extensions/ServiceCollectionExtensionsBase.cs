using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

namespace AgriTech.Extensions;
public static class ServiceCollectionExtensions
{
    public static void ConfigureApp(this IApplicationBuilder app)
    {
        app.UseResponseCompression();
        app.UseSerilogRequestLogging();

        app.ConfigureExceptionHandler();

        app.UseSecurityHeaders();
        app.ConfigureAntiforgery();

        app.UseHttpsRedirection();

        // app.UseRequestMiddleware();
        app.UseCookiePolicy();

        app.UseCors(builder =>
        {
            builder.WithOrigins(
                    "http://localhost:4200",
                    "http://localhost:3000",
                    "https://localhost:44390",
                    "https://www.agritech.com",
                    "https://dev.agritech.com",
                    "https://devcms.agritech.com",
                    "https://cms.agritech.com",
                    "https://*.agritech.com",
                    "https://agritech.com",
                    "http://www.agritech.com",
                    "http://dev.agritech.com",
                    "https://devapi.agritech.com",
                    "https://api.agritech.com",
                    "http://agritech.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
    }
}