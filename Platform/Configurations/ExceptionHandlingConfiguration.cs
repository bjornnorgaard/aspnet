using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Platform.Middleware;

namespace Platform.Configurations;

public static class ExceptionHandlingConfiguration
{
    public static void AddPlatformExceptionHandling(this IHostApplicationBuilder builder)
    {
        // builder.Services.AddTransient<GlobalExceptionMiddleware>();
    }

    public static void UsePlatformExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}