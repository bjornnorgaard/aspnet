using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoByIdEndpoint(ILogger<GetTodoByIdEndpoint> logger) : AbstractEndpoint
{
    public override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Get,
        Version = Versions.V1,
        Route = "get-todo-by-id/{id:guid}",
        Description = "Retrieves a todo item by ID."
    };

    public override Delegate Handle => (Guid id) =>
    {
        logger.LogInformation("Handling GetTodoById request with id: {Id}", id);
        return Results.Ok($"Todo item retrieved - ID: {id}.");
    };
}