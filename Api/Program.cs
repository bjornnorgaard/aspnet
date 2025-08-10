using Api;
using Api.Database;
using Api.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform;

var anchor = typeof(AssemblyAnchor).Assembly;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<Context>(o => o.UseNpgsql(cs));

builder.AddPlatform(anchor);

var app = builder.Build();

// Only run database migrations if not in test mode
if (app.Environment.EnvironmentName != "Testing")
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        await context.Database.MigrateAsync();
    }
}

app.UsePlatform(anchor);

app.MapGet("test", async ([FromServices] Context context) =>
{
    var created = await context.Todos.AddAsync(new Todo
    {
        Title = "Test Todo",
        Description = "This is a test todo item.",
        IsCompleted = false,
        CreatedAt = DateTimeOffset.UtcNow,
    });

    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        created.Entity.Id,
        created.Entity.Title,
        created.Entity.Description,
        created.Entity.IsCompleted,
        created.Entity.CreatedAt
    });
});

app.Run();