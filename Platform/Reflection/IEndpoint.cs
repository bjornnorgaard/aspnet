using Microsoft.AspNetCore.Http;

namespace Platform.Reflection;

public interface IEndpoint
{
    public EndpointDefinition Definition();
    public IResult Handle();
}