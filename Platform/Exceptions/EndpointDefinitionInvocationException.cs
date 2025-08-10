using Platform.Reflection;

namespace Platform.Exceptions;

public class EndpointDefinitionInvocationException : Exception
{
    public EndpointDefinitionInvocationException(Type type)
    {
        var message =
            $"The static method '{nameof(AbstractEndpoint.Definition)}' from the type {type.FullName} returned null. Ensure it returns a valid endpoint definition.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}