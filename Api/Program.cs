using Api;
using Api.Endpoints;
using Api.Endpoints.Todos;
using Platform;

var builder = WebApplication.CreateBuilder(args);

var anchor = typeof(AssemblyAnchor).Assembly;

builder.AddPlatform(anchor);

builder.Services.AddTransient<GetTodoEndpoint>();

var app = builder.Build();

app.UsePlatform(anchor);

// app.MapGroup(Versions.V1)
//     .MapGroup(Groups.Todos)
//     .MapGet(Routes.GetTodo, (GetTodoEndpoint e) => e.Handle());

app.Run();