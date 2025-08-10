using Microsoft.AspNetCore.Http;

namespace Platform.Reflection;

public interface IEndpoint
{
    public static abstract EndpointDefinition Definition();
    public IResult Handle();
}

