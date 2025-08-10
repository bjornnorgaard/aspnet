using Api.Endpoints.Todos;
using Platform;

var builder = WebApplication.CreateBuilder(args);

builder.AddPlatform();

builder.Services.AddTransient<GetTodoEndpoint>();

var app = builder.Build();

app.UsePlatform();

app.MapGroup("todos")
    .MapGet("get-todo", (GetTodoEndpoint e) => e.Handle());

app.Run();