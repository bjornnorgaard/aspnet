using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform.Reflection;

namespace Api.Features.Todos;

public class GetTodoList
{
    public class Request
    {
        public int Limit { get; set; }
    }

    public class Result
    {
        public required List<Todo> List { get; init; } = [];

        public class Todo
        {
            public Guid Id { get; set; }
            public string? Title { get; set; }
            public bool IsCompleted { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
        }
    }

    public class Handler(Context context) : AbstractFeature
    {
        public async Task<Result> Handle(Request request, CancellationToken ct)
        {
            var todos = await context.Todos.AsNoTracking()
                .Take(request.Limit)
                .Select(t => new Result.Todo
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync(ct);

            return new Result { List = todos };
        }
    }
}