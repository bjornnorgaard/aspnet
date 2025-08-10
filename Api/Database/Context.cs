using Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class Context(DbContextOptions options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
    }
}