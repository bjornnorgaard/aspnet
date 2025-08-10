namespace Platform.Reflection;

public class EndpointDefinition
{
    public required string Route { get; init; }
    public required HttpMethod Method { get; init; }
    public required string Version { get; init; }
    public required string Group { get; init; }
    public string? Description { get; init; }
}