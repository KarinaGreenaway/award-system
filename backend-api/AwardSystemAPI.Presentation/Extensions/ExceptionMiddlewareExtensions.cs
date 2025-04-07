using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace AwardSystemAPI.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    HttpStatusCode statusCode;
                    
                    var exception = contextFeature.Error;
                    switch (exception)
                    {
                        case UnauthorizedAccessException _:
                            statusCode = HttpStatusCode.Unauthorized;
                            break;
                        case ValidationException _:
                            statusCode = HttpStatusCode.BadRequest;
                            break;
                        default:
                            statusCode = HttpStatusCode.InternalServerError;
                            break;
                    }
                    context.Response.StatusCode = (int)statusCode;
                    context.Response.ContentType = "application/json";

                    var problemDetails = new
                    {
                        Title = "An error occurred while processing your request.",
                        Status = (int)statusCode,
                        
                        // Showing error details only in development
                        Detail = env.IsDevelopment() ? contextFeature.Error.Message : "Internal Server Error. Please try again later.",
                        TraceId = context.TraceIdentifier
                    };

                    logger.LogError(contextFeature.Error, "An unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);

                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
                }
            });
        });
    }
}