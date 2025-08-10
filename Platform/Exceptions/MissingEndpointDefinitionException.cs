using Platform.Reflection;

namespace Platform.Exceptions;

public class MissingEndpointDefinitionException : Exception
{
    public MissingEndpointDefinitionException(Type type)
    {
        var message =
            $"The type {type.FullName} does not implement the required static method '{nameof(AbstractEndpoint.Definition)}' from the {nameof(AbstractEndpoint)} interface.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}