using Platform.Reflection;

namespace Platform.Exceptions;

public class EndpointHandlerNotFoundException : Exception
{
    public EndpointHandlerNotFoundException(Type type)
    {
        var message =
            $"The type {type.FullName} does not implement the required instance method '{nameof(AbstractEndpoint.Handle)}'.";
        Console.WriteLine(message);
        throw new InvalidOperationException(message);
    }
}