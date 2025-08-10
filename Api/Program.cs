using Api;
using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform;

var anchor = typeof(AssemblyAnchor).Assembly;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContextPool<Context>(o => o.UseNpgsql(cs));

builder.AddPlatform(anchor);

var app = builder.Build();
app.UsePlatform(anchor);

app.Run();