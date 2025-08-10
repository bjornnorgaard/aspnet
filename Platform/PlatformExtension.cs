using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Platform.Configurations;

namespace Platform;

public static class PlatformExtension
{
    public static IHostApplicationBuilder AddPlatform(this IHostApplicationBuilder builder)
    {
        builder.AddPlatformOpenApi();
        return builder;
    }

    public static WebApplication UsePlatform(this WebApplication app)
    {
        app.MapPlatformOpenApi();
        return app;
    }
}