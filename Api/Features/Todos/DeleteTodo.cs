using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform.Reflection;

namespace Api.Features.Todos;

public class DeleteTodo
{
    public class Command
    {
        public Guid Id { get; init; }
    }

    public class Result
    {
        public bool Success { get; init; }
    }

    public class Handler(Context context) : AbstractFeature
    {
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            var rowsAffected = await context.Todos
                .Where(t => t.Id == command.Id)
                .ExecuteDeleteAsync(ct);

            return new Result { Success = rowsAffected > 0 };
        }
    }
}