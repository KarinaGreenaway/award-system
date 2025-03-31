using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace AwardSystemAPI.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger.LogError(contextFeature.Error, "An unhandled exception occurred.");

                    var errorResponse = new { Message = "Internal Server Error. Please try again later." };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                }
            });
        });
    }
}