using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Platform.Configurations;

namespace Platform;

public static class PlatformExtension
{
    public static IHostApplicationBuilder AddPlatform(this IHostApplicationBuilder builder, Assembly anchor)
    {
        builder.AddPlatformOpenApi();
        builder.AddPlatformEndpoints(anchor);
        return builder;
    }

    public static WebApplication UsePlatform(this WebApplication app, Assembly anchor)
    {
        app.MapPlatformOpenApi();
        app.MapPlatformEndpoints(anchor);
        return app;
    }
}