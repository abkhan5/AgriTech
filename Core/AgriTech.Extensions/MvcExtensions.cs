using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AgriTech.Extensions;

public static class MvcExtensions
{

    public static string GetUserId(this HttpContext httpcontext)
    {
        return httpcontext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    }


    public static void AddApiControllers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAntiforgery(options => { options.HeaderName = "X-XSRF-TOKEN"; });
        services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 268435456; });
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.AddRouting(options =>
        {
            // options.LowercaseUrls = true;
            options.AppendTrailingSlash = false;
            options.LowercaseQueryStrings = false;
        });
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
        services
            .AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.SuppressAsyncSuffixInActionNames = true;


            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                // options.JsonSerializerOptions.Converters.Add(new DescriptionConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {

                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };
                    var result = new BadRequestObjectResult(problemDetails);
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                    return result;
                };
            });
    }

    public static void ConfigureAntiforgery(this IApplicationBuilder app)
    {
        app.Use(next => context =>
        {
            var path = context.Request.Path.Value;
            if (path.Contains("/api"))
            {
                var antiforgery = context.RequestServices.GetService<IAntiforgery>();
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            }
            return next(context);
        });
    }
}