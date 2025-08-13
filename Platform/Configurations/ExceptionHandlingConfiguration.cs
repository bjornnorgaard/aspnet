using Microsoft.AspNetCore.Builder;
using Platform.Middleware;

namespace Platform.Configurations;

public static class ExceptionHandlingConfiguration
{
    public static void UsePlatformExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}