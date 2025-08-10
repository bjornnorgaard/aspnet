using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Platform.Reflection;

public abstract class AbstractEndpoint
{
    protected abstract EndpointDefinition Definition();

    protected abstract Delegate Handle { get; }

    public virtual void Register(WebApplication app)
    {
        var d = Definition();
        app.MapGroup(d.Version)
            .MapGroup(d.Group)
            .WithTags(d.Group, d.Version)
            .WithDescription(d.Description ?? $"Endpoint for {d.Version} {d.Group} {d.Route}")
            .MapMethods(d.Route, [d.Method.ToString()], Handle);
    }
}