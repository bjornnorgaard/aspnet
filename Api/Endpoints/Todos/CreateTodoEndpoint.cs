using Api.Endpoints.Constants;
using Application.Features.Todos;
using Microsoft.AspNetCore.Mvc;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class CreateTodoEndpoint : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Post,
        Version = Versions.V1,
        Route = "create-todo",
        Description = "Creates a new todo item."
    };

    public record Request(string Title, string Description);

    protected override Delegate Handle => async (
        [FromServices] CreateTodo.Handler feature,
        [FromBody] Request request,
        CancellationToken ct) =>
    {
        var command = new CreateTodo.Command { Title = request.Title, Description = request.Description };
        var result = await feature.Handle(command, ct);

        return Results.Created($"/get-todo-by-id/{result.Todo.Id}", result.Todo);
    };
}