using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Platform.Middleware;

namespace Platform.Configurations;

public static class ExceptionHandlingConfiguration
{
    public static IHostApplicationBuilder AddPlatformExceptionHandling(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<GlobalExceptionMiddleware>();
        return builder;
    }

    public static WebApplication UsePlatformExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}