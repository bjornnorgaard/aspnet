using Platform.Reflection;

namespace Api.Endpoints.Todos;

public class GetTodoListEndpoint(ILogger<GetTodoListEndpoint> logger) : AbstractEndpoint
{
    public override EndpointDefinition Definition()
    {
        return new EndpointDefinition
        {
            Group = Groups.Todos,
            Method = HttpMethod.Get,
            Version = Versions.V1,
            Route = Routes.GetTodoList,
            Description = "Retrieves all todo items."
        };
    }

    public override IResult Handle()
    {
        logger.LogInformation("Handling GetTodos request.");
        return Results.Ok("All todo items retrieved successfully.");
    }
}