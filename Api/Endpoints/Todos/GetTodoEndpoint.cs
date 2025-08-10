namespace Api.Endpoints.Todos;

public class GetTodoEndpoint(ILogger<GetTodoEndpoint> logger)
{
    public IResult Handle()
    {
        logger.LogInformation("Handling GetTodoEndpoint request");
        return Results.Ok(new { Message = "Todo item retrieved successfully" });
    }
}