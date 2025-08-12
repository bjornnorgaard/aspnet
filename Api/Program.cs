using Api;
using Api.Database;
using Api.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Platform;

var builder = WebApplication.CreateBuilder(args);
Platform.Platform.AddServices(builder);
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<Context>(o => o.UseNpgsql(cs));

var app = builder.Build();
Platform.Platform.UsePlatform(app);
Platform.Platform.MigrateDatabase<Context>(app);

app.Run();