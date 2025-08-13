using Microsoft.Extensions.Configuration;

namespace Platform.Options;

public abstract class AbstractOption
{
    protected AbstractOption(IConfiguration configuration)
    {
        var typeName = GetType().Name;
        configuration.GetSection(typeName).Bind(this);
    }
}