using Api.Database;
using Api.Database.Models;
using Platform.Reflection;

namespace Api.Features.Todos;

public class CreateTodo
{
    public class Command
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }

    public class Result
    {
        public required TodoItem Todo { get; init; }

        public class TodoItem
        {
            public Guid Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public bool IsCompleted { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
        }
    }

    public class Handler(Context context) : AbstractFeature
    {
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var todo = new Todo
            {
                Title = command.Title,
                Description = command.Description,
                IsCompleted = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await context.Todos.AddAsync(todo, ct);
            await context.SaveChangesAsync(ct);

            return new Result
            {
                Todo = new Result.TodoItem
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    IsCompleted = todo.IsCompleted,
                    CreatedAt = todo.CreatedAt
                }
            };
        }
    }
}