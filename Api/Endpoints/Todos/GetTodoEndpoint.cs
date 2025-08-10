using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoEndpoint(ILogger<GetTodoEndpoint> logger) : IEndpoint
{
    public EndpointDefinition Definition()
    {
        return new EndpointDefinition
        {
            Group = Groups.Todos,
            Version = Versions.V1,
            Route = Routes.GetTodo,
        };
    }

    public IResult Handle()
    {
        return Results.Ok("Todo item retrieved successfully.");
    }
}