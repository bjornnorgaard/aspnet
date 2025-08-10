using Api;
using Platform;

var builder = WebApplication.CreateBuilder(args);

var anchor = typeof(AssemblyAnchor).Assembly;

builder.AddPlatform(anchor);

var app = builder.Build();

app.UsePlatform(anchor);

app.Run();