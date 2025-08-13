using Api.Endpoints.Constants;
using Application.Features.Todos;
using Microsoft.AspNetCore.Mvc;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoByIdEndpoint : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Get,
        Version = Versions.V1,
        Route = "get-todo-by-id/{id:guid}",
        Description = "Retrieves a todo item by ID."
    };

    protected override Delegate Handle => async (
        [FromServices] GetTodoById.Handler feature,
        [FromRoute] Guid id,
        CancellationToken ct) =>
    {
        if (id == Guid.Empty)
        {
            return Results.BadRequest("Invalid GUID format");
        }

        var command = new GetTodoById.Command { Id = id };
        var result = await feature.Handle(command, ct);

        if (result.Todo == null)
        {
            return Results.NotFound($"Todo with ID {id} not found.");
        }

        return Results.Ok(result.Todo);
    };
}