using Api.Endpoints.Constants;
using Api.Features.Todos;
using Microsoft.AspNetCore.Mvc;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoListEndpoint : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Post,
        Version = Versions.V1,
        Route = "get-todo-list",
        Description = "Retrieves all todo items."
    };

    public record Request(int Limit = 10, Guid? Cursor = null);

    protected override Delegate Handle => async (
        [FromServices] GetTodoList.Handler feature,
        [FromBody] Request request,
        CancellationToken ct) =>
    {
        var command = new GetTodoList.Command
        {
            Limit = request.Limit,
            Cursor = request.Cursor
        };
        var result = await feature.Handle(command, ct);
        return Results.Ok(result);
    };
}