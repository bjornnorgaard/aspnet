using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Platform.Options;

namespace Platform.Configurations;

public static class TelemetryConfiguration
{
    public static void AddPlatformTelemetry(this IHostApplicationBuilder builder)
    {
        var opts = new TelemetryOptions(builder.Configuration);
        var serviceInstanceId = $"{Environment.MachineName.ToLower()} {DateTime.Now:HHmmss}";

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: opts.ServiceName,
                    serviceVersion: opts.AppVersion,
                    serviceNamespace: opts.Namespace,
                    serviceInstanceId: serviceInstanceId))
            .WithTracing(tracing => tracing
                .AddSource(Telemetry.SourceName)
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
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
                    serviceName: opts.ServiceName,
                    serviceVersion: opts.AppVersion,
                    serviceNamespace: opts.Namespace,
                    serviceInstanceId: serviceInstanceId))
                .AddOtlpExporter();
        });
    }
}

public static class Telemetry
{
    public const string SourceName = "Platform.Telemetry";
    public static ActivitySource Source = new(SourceName);
}