using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Configurations;

namespace Platform;

public static class PlatformExtensions
{
    public static void AddPlatform(this WebApplicationBuilder builder)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        builder.AddPlatformOpenApi();
        builder.AddPlatformTelemetry();
        builder.AddPlatformFeatures(callingAssembly);
        builder.AddPlatformEndpoints(callingAssembly);
    }

    public static void UsePlatform(this WebApplication app)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        app.UsePlatformExceptionHandling();
        app.MapPlatformOpenApi();
        app.MapPlatformEndpoints(callingAssembly);
    }

    public static void MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}