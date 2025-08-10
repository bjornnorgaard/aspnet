using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoEndpoint(ILogger<GetTodoEndpoint> logger) : IEndpoint
{
    public static EndpointDefinition Definition()
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
        logger.LogInformation("Handling GetTodo request.");
        return Results.Ok("Todo item retrieved successfully.");
    }
}