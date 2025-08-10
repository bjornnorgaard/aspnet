using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform.Reflection;

namespace Api.Features.Todos;

public class GetTodoById
{
    public class Command
    {
        public Guid Id { get; set; }
    }

    public class Result
    {
        public required TodoItem? Todo { get; init; }

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
            var todo = await context.Todos.AsNoTracking()
                .Where(t => t.Id == command.Id)
                .Select(t => new Result.TodoItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            return new Result { Todo = todo };
        }
    }
}