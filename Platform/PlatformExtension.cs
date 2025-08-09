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
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }
}