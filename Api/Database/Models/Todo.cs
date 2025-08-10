using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Database.Models;

public class Todo
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsCompleted { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }

    public class Configuration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired();
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.IsCompleted).IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();
        }
    }
}