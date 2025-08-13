using Api.Endpoints.Constants;
using Application.Features.Todos;
using Microsoft.AspNetCore.Mvc;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class UpdateTodoEndpoint : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Put,
        Version = Versions.V1,
        Route = "update-todo/{id:guid}",
        Description = "Updates an existing todo item."
    };

    public record Request(string Title, string Description, bool IsCompleted);

    protected override Delegate Handle => async (
        [FromServices] UpdateTodo.Handler feature,
        [FromRoute] Guid id,
        [FromBody] Request request,
        CancellationToken ct) =>
    {
        if (id == Guid.Empty)
        {
            return Results.BadRequest("Invalid GUID format");
        }

        var command = new UpdateTodo.Command
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted
        };

        var result = await feature.Handle(command, ct);
        if (!result.Success)
        {
            return Results.NotFound($"Todo with id {id} not found");
        }

        return Results.NoContent();
    };
}