using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Platform;

public static class PlatformExtension
{
    public static IHostApplicationBuilder AddPlatform(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication UsePlatform(this WebApplication app)
    {
        app.MapOpenApi();

        RegisterEndpoints(app);

        return app;
    }

    /// <summary>
    /// Uses reflection to register all endpoints that inherit from PlatformEndpoint.
    /// </summary>
    /// <param name="app"></param>
    private static void RegisterEndpoints(WebApplication app)
    {
        var endpointTypes = typeof(PlatformEndpoint).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(PlatformEndpoint)) && !t.IsAbstract);

        foreach (var endpointType in endpointTypes)
        {
            var endpointInstance = (PlatformEndpoint)Activator.CreateInstance(endpointType)!;
            endpointInstance.Register(app);
        }
    }
}