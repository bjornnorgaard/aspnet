using Api.Database;
using Microsoft.EntityFrameworkCore;
using Platform.Reflection;

namespace Api.Features.Todos;

public class UpdateTodo
{
    public class Command
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class Result
    {
        public bool Success { get; init; }
    }

    public class Handler(Context context) : AbstractFeature
    {
        public async Task<Result> Handle(Command command, CancellationToken ct)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(command.Title))
            {
                throw new ArgumentException("Title cannot be empty", nameof(command.Title));
            }

            if (string.IsNullOrWhiteSpace(command.Description))
            {
                throw new ArgumentException("Description cannot be empty", nameof(command.Description));
            }

            var rowsAffected = await context.Todos
                .Where(t => t.Id == command.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.Title, command.Title)
                    .SetProperty(t => t.Description, command.Description)
                    .SetProperty(t => t.IsCompleted, command.IsCompleted), ct);

            return new Result { Success = rowsAffected > 0 };
        }
    }
}