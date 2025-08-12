using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Platform.Configurations;

public static class TelemetryConfiguration
{
    public static void AddPlatformTelemetry(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Environment.ApplicationName;
        var serviceVersion = "1.0.0"; // You can set this dynamically or from configuration
        var serviceNamespace = "team-name"; // You can set this dynamically or from configuration
        var instanceId = Environment.MachineName; // You can set this dynamically or from configuration

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion,
                    serviceNamespace: serviceNamespace,
                    serviceInstanceId: instanceId))
            .WithTracing(tracing => tracing
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter()
            );

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion,
                    serviceNamespace: serviceNamespace,
                    serviceInstanceId: instanceId))
                .AddConsoleExporter()
                .AddOtlpExporter();
        });
    }

    public static void MapPlatformTelemetry(this WebApplication app)
    {
    }
}