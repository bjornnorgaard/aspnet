using Api;
using Platform;

var anchor = typeof(AssemblyAnchor).Assembly;

var builder = WebApplication.CreateBuilder(args);
builder.AddPlatform(anchor);

var app = builder.Build();
app.UsePlatform(anchor);

app.Run();