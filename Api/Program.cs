using Api;
using Api.Endpoints.Todos;
using Platform;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<GetTodoByIdEndpoint>();

var anchor = typeof(AssemblyAnchor).Assembly;

builder.AddPlatform(anchor);

var app = builder.Build();

app.UsePlatform(anchor);

app.Run();