using Microsoft.Extensions.Configuration;

namespace Platform.Options;

public class TelemetryOptions(IConfiguration configuration) : AbstractOption(configuration)
{
    public  string ServiceName { get; set; } = null!;
    public  string AppVersion { get; set; } = null!;
    public  string Namespace { get; set; } = null!;
}
