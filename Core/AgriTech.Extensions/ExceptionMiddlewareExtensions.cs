using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace AgriTech.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                // https://tools.ietf.org/html/rfc7807#section-3.1
                var problemDetails = new ProblemDetails
                {
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                    //Type = $"https://example.com/problem-types/{exception.GetType().Name}",
                };
                Dictionary<string, IEnumerable<string>> validationErrors = null;
                switch (exception)
                {
                   

                    case ValidationException validationException:
                        validationErrors = validationException.FormatValidationException();
                        problemDetails.Status = validationException.GetStatusCodes();
                        break;

                    case Exception generalException:
                        validationErrors = generalException.FormatValidationException();
                        break;
                }
                var redirectUrl = GetRedirect(problemDetails.Status);
                if (string.IsNullOrEmpty(redirectUrl))
                    context.Response.Redirect(redirectUrl);

                problemDetails.Extensions["errors"] = validationErrors;
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problemDetails.Status.Value;
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };
                await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);
            });
        });
    }
    private static string GetRedirect(int? httpCode) =>
        httpCode switch
        {
            StatusCodes.Status403Forbidden or StatusCodes.Status410Gone or StatusCodes.Status401Unauthorized
            => "https://www.everyeng.com",
            _ => "",
        };



    private static int GetStatusCodes(this ValidationException validationException)
    {       

        return StatusCodes.Status400BadRequest;
    }


    private static Dictionary<string, IEnumerable<string>> FormatValidationException(this Exception everyEngException) =>
        new()
        {
            [""] = new List<string> { everyEngException.Message }
        };


    #region Fluent validation

    
    #endregion
}
