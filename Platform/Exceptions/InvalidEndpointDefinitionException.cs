using Platform.Reflection;

namespace Platform.Exceptions;

public class InvalidEndpointDefinitionException : Exception
{
    public InvalidEndpointDefinitionException(Type type, Type definitionType)
    {
        var message =
            $"The static method '{nameof(AbstractEndpoint.Definition)}' from the type {type.FullName} returned an invalid type. Expected {nameof(EndpointDefinition)}, but got {definitionType.FullName}.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}