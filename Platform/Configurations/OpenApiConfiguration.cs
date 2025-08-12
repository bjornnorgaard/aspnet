using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Platform.Configurations;

public static class OpenApiConfiguration
{
    public static void AddPlatformOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
    }

    public static void MapPlatformOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}