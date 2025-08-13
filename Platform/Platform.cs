using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Configurations;

namespace Platform;

public static class Platform
{
    public static void AddServices(WebApplicationBuilder builder)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        builder.AddPlatformExceptionHandling();
        builder.AddPlatformOpenApi();
        builder.AddPlatformTelemetry();
        builder.AddPlatformOptions(callingAssembly, builder.Configuration);
        builder.AddPlatformFeatures(callingAssembly);
        builder.AddPlatformEndpoints(callingAssembly);
    }

    public static void UsePlatform(WebApplication app)
    {
        app.UsePlatformExceptionHandling();
        app.MapPlatformOpenApi();
        app.MapPlatformTelemetry();
        app.MapPlatformEndpoints(Assembly.GetCallingAssembly());
    }

    public static void MigrateDatabase<T>(WebApplication app) where T: DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}