using Api;
using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform;

var anchor = typeof(AssemblyAnchor).Assembly;

var builder = WebApplication.CreateBuilder(args);
builder.AddPlatform(anchor);

var cs = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContextPool<Context>(o => o.UseNpgsql(cs));

var app = builder.Build();
app.UsePlatform(anchor);

app.Run();