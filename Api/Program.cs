using Application.Database;
using Microsoft.EntityFrameworkCore;
using Platform;

var builder = WebApplication.CreateBuilder(args);
builder.AddPlatform();
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<Context>(o => o.UseNpgsql(cs));

var app = builder.Build();
app.UsePlatform();
app.MigrateDatabase<Context>();

app.Run();