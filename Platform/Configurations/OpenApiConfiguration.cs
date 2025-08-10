using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Platform.Configurations;

public static class OpenApiConfiguration
{
    public static IHostApplicationBuilder AddPlatformOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication MapPlatformOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        return app;
    }
}