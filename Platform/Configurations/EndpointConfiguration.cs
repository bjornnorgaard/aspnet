using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Platform.Configurations;

public static class EndpointConfiguration
{
    public static IHostApplicationBuilder AddPlatformEndpoints(this IHostApplicationBuilder builder, Assembly anchor)
    {
        return builder;
    }

    public static WebApplication MapPlatformEndpoints(this WebApplication app, Assembly anchor)
    {
        return app;
    }
}