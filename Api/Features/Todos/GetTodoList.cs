using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform.Reflection;

namespace Api.Features.Todos;

public class GetTodoList
{
    public class Command
    {
        public int Limit { get; set; } = 10;
        public Guid? Cursor { get; set; }
    }

    public class Result
    {
        public required List<Todo> List { get; init; } = [];
        public Guid? NextCursor { get; init; }
        public bool HasMore { get; init; }

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
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var query = context.Todos.AsNoTracking();

            if (command.Cursor.HasValue)
            {
                query = query.Where(t => t.Id > command.Cursor.Value);
            }

            var todos = await query
                .OrderBy(t => t.Id)
                .Take(command.Limit + 1)
                .Select(t => new Result.Todo
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync(ct);

            var hasMore = todos.Count > command.Limit;
            if (hasMore)
            {
                todos.RemoveAt(todos.Count - 1);
            }

            Guid? nextCursor = null;
            if (hasMore && todos.Count > 0)
            {
                nextCursor = todos.Last().Id;
            }

            return new Result
            {
                List = todos,
                NextCursor = nextCursor,
                HasMore = hasMore
            };
        }
    }
}