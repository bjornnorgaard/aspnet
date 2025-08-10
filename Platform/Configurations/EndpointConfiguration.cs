using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Platform.Reflection;

namespace Platform.Configurations;

public static class EndpointConfiguration
{
    public static IHostApplicationBuilder AddPlatformEndpoints(this IHostApplicationBuilder builder, Assembly anchor)
    {
        return builder;
    }

    public static WebApplication MapPlatformEndpoints(this WebApplication app, Assembly anchor)
    {
        var types = anchor.GetTypes().Where(t => t.IsAssignableTo(typeof(IEndpoint)));
        foreach (var type in types)
        {
            var definitionMethod = type.GetMethods()
                .FirstOrDefault(m => m.Name == nameof(IEndpoint.Definition));

            if (definitionMethod == null)
            {
                throw new MissingEndpointDefinitionException(type);
            }

            var definition = definitionMethod.Invoke(null, null);
            if (definition == null)
            {
                throw new EndpointDefinitionInvocationException(type);
            }

            if (definition is not EndpointDefinition endpointDefinition)
            {
                throw new InvalidEndpointDefinitionException(type, definition.GetType());
            }

            var handlerMethod = type.GetMethod(nameof(IEndpoint.Handle), BindingFlags.Public | BindingFlags.Instance);
            if (handlerMethod == null)
            {
                throw new EndpointHandlerNotFoundException(type);
            }

            app.MapGroup(endpointDefinition.Version)
                .MapGroup(endpointDefinition.Group)
                .MapGet(endpointDefinition.Route, (IEndpoint endpoint) => handlerMethod.Invoke(endpoint, null));
        }

        return app;
    }
}

public class MissingEndpointDefinitionException : Exception
{
    public MissingEndpointDefinitionException(Type type)
    {
        var message =
            $"The type {type.FullName} does not implement the required static method '{nameof(IEndpoint.Definition)}' from the {nameof(IEndpoint)} interface.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}

public class EndpointDefinitionInvocationException : Exception
{
    public EndpointDefinitionInvocationException(Type type)
    {
        var message =
            $"The static method '{nameof(IEndpoint.Definition)}' from the type {type.FullName} returned null. Ensure it returns a valid endpoint definition.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}

public class InvalidEndpointDefinitionException : Exception
{
    public InvalidEndpointDefinitionException(Type type, Type definitionType)
    {
        var message =
            $"The static method '{nameof(IEndpoint.Definition)}' from the type {type.FullName} returned an invalid type. Expected {nameof(EndpointDefinition)}, but got {definitionType.FullName}.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}

public class EndpointHandlerNotFoundException : Exception
{
    public EndpointHandlerNotFoundException(Type type)
    {
        var message =
            $"The type {type.FullName} does not implement the required instance method '{nameof(IEndpoint.Handle)}'.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}