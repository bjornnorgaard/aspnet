using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Platform.Options;

namespace Platform.Configurations;

public static class OptionsConfiguration
{
    public static void AddPlatformOptions(this IHostApplicationBuilder builder, Assembly anchor, IConfiguration configuration)
    {
        var endpointTypes = anchor.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(AbstractOption)) && !type.IsAbstract)
            .ToArray();

        foreach (var optionsType in endpointTypes)
        {
            builder.Services.AddOptions(optionsType)
                .Bind(configuration.GetSection(optionsType.Name))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}