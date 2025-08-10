using Api.Endpoints.Constants;
using Api.Features.Todos;
using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoListEndpoint(GetTodoList.Handler feature) : AbstractEndpoint
{
    protected override EndpointDefinition Definition() => new()
    {
        Group = Groups.Todos,
        Method = HttpMethod.Get,
        Version = Versions.V1,
        Route = "get-todo-list",
        Description = "Retrieves all todo items."
    };

    protected override Delegate Handle => async (int limit) =>
    {
        var request = new GetTodoList.Request { Limit = limit };
        var result = await feature.Handle(request, CancellationToken.None);
        return Results.Ok(result.List);
    };
}