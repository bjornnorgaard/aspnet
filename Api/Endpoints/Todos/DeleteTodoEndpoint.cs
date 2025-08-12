using Api.Endpoints.Constants;
using Api.Features.Todos;
using Microsoft.AspNetCore.Mvc;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class DeleteTodoEndpoint : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Delete,
        Version = Versions.V1,
        Route = "delete-todo/{id:guid}",
        Description = "Deletes an existing todo item."
    };

    protected override Delegate Handle => async (
        [FromServices] DeleteTodo.Handler feature,
        [FromRoute] Guid id,
        CancellationToken ct) =>
    {
        if (id == Guid.Empty)
        {
            return Results.BadRequest("Invalid GUID format");
        }

        var command = new DeleteTodo.Command { Id = id };
        var result = await feature.Handle(command, ct);

        if (!result.Success)
        {
            return Results.NotFound($"Todo with id {id} not found");
        }

        return Results.NoContent();
    };
}