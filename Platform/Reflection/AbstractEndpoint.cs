using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Platform.Reflection;

public abstract class AbstractEndpoint
{
    public abstract EndpointDefinition Definition();

    public abstract Delegate Handle();

    public virtual void Register(WebApplication app)
    {
        var d = Definition();
        app.MapGroup(d.Version)
            .MapGroup(d.Group)
            .WithTags(d.Group, d.Version)
            .WithName($"Route name: {d.Route}")
            .WithDescription(d.Description ?? $"Endpoint for {d.Group} {d.Version} {d.Route}")
            .MapMethods(d.Route, [d.Method.ToString()], Handle());
    }
}