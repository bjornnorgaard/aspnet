using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Platform.Reflection;

namespace Platform.Configurations;

public static class FeatureConfiguration
{
    public static IHostApplicationBuilder AddPlatformFeatures(this IHostApplicationBuilder builder, Assembly anchor)
    {
        var featureTypes = anchor.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(AbstractFeature)) && !type.IsAbstract)
            .ToArray();

        foreach (var featureType in featureTypes)
        {
            builder.Services.AddTransient(featureType);
        }

        return builder;
    }
}