using Platform.Reflection;

namespace Api.Endpoints.Todos;

public record CreateTodoRequest(string Title, string Description);

public class CreateTodoEndpoint(ILogger<CreateTodoEndpoint> logger) : AbstractEndpoint
{
    public override EndpointDefinition Definition()
    {
        return new EndpointDefinition
        {
            Group = Groups.Todos,
            Method = HttpMethod.Post,
            Version = Versions.V1,
            Route = Routes.CreateTodo,
            Description = "Creates a new todo item."
        };
    }

    public override Delegate Handle()
    {
        return (CreateTodoRequest request) =>
        {
            logger.LogInformation("Creating new todo: {Title}", request.Title);
            return Results.Created($"/api/v1/todos/{Guid.NewGuid()}", new
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            });
        };
    }
}