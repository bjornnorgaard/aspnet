using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoEndpoint(ILogger<GetTodoEndpoint> logger) : IEndpoint
{
    public EndpointDefinition Definition()
    {
        return new EndpointDefinition
        {
            Group = EndpointGroup.Todos,
            Version = EndpointVersion.V1,
            Route = EndpointRoutes.GetTodo,
        };
    }

    public IResult Handle()
    {
        throw new NotImplementedException();
    }
}