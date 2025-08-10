using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoEndpoint(ILogger<GetTodoEndpoint> logger) : AbstractEndpoint
{
    public override EndpointDefinition Definition()
    {
        return new EndpointDefinition
        {
            Group = Groups.Todos,
            Method = HttpMethod.Get,
            Version = Versions.V1,
            Route = Routes.GetTodo,
            Description = "Retrieves a todo item by ID."
        };
    }

    public override Delegate Handle()
    {
        return (int id) =>
        {
            logger.LogInformation("Handling GetTodo request for ID: {Id}", id);
            return Results.Ok($"Todo item {id} retrieved successfully.");
        };
    }
}