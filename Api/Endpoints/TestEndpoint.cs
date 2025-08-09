using Platform;

namespace Api.Endpoints;

public class TestEndpoint : PlatformEndpoint
{
    public override void Register(WebApplication app)
    {
        app.MapGet("/test", Handler)
            .WithName("TestEndpoint")
            .WithOpenApi();
    }

    private static string Handler(HttpContext context)
    {
        return "hello world";
    }
}