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
        Method = HttpMethod.Post,
        Version = Versions.V1,
        Route = "delete-todo",
        Description = "Deletes an existing todo item."
    };

    public record DeleteTodoRequest(Guid Id);

    protected override Delegate Handle => async (
        [FromServices] DeleteTodo.Handler feature,
        [FromBody] DeleteTodoRequest request,
        CancellationToken ct) =>
    {
        var command = new DeleteTodo.Command { Id = request.Id };
        var result = await feature.Handle(command, ct);

        if (!result.Success)
        {
            return Results.NotFound($"Todo with ID {request.Id} not found.");
        }

        return Results.NoContent();
    };
}