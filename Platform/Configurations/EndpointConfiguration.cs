using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Platform.Reflection;

namespace Platform.Configurations;

public static class EndpointConfiguration
{
    public static void AddPlatformEndpoints(this IHostApplicationBuilder builder, Assembly anchor)
    {
        var endpointTypes = anchor.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(AbstractEndpoint)) && !type.IsAbstract)
            .ToArray();

        foreach (var endpointType in endpointTypes)
        {
            builder.Services.AddTransient(endpointType);
        }
    }

    public static void MapPlatformEndpoints(this WebApplication app, Assembly anchor)
    {
        var endpointTypes = anchor.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(AbstractEndpoint)) && !type.IsAbstract)
            .ToArray();

        foreach (var endpointType in endpointTypes)
        {
            var endpoint = (AbstractEndpoint)app.Services.GetRequiredService(endpointType);
            endpoint.Register(app);
        }
    }
}