using Microsoft.AspNetCore.Builder;

namespace Platform;

public abstract class PlatformEndpoint
{
    /// <summary>
    /// Method to register the endpoint with the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    public abstract void Register(WebApplication app);
}