using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Platform.Reflection;

public abstract class AbstractEndpoint
{
    public abstract EndpointDefinition Definition();

    public abstract IResult Handle();

    public virtual void Register(WebApplication app)
    {
        var d = Definition();
        app.MapGroup(d.Version)
            .MapGroup(d.Group)
            .MapGet(d.Route, Handle);
    }
}