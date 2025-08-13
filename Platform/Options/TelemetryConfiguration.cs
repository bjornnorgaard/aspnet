using System.ComponentModel.DataAnnotations;

namespace Platform.Options;

public class TelemetryConfiguration : AbstractOption
{
    [Required]
    public required string ServiceName { get; set; }

    [Required]
    public required string AppVersion { get; set; }

    [Required]
    public required string Namespace { get; set; }
}

public abstract class AbstractOption
{

}

