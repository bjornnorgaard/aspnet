using Api.Endpoints.Constants;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoListEndpoint(ILogger<GetTodoListEndpoint> logger) : AbstractEndpoint
{
    public override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Get,
        Version = Versions.V1,
        Route = "get-todo-list",
        Description = "Retrieves all todo items."
    };

    public override Delegate Handle => (int page, int pageSize) =>
    {
        logger.LogInformation("Handling GetTodos request with page: {Page}, pageSize: {PageSize}", page, pageSize);
        return Results.Ok($"Todo items retrieved - page {page}, size {pageSize}.");
    };
}