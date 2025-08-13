using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Database;
using Application.Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests;

public class GetTodoListIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GetTodoListIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTodoList_WithDefaultParameters_ShouldReturnTodos()
    {
        // Arrange - Create multiple todos
        await CreateTestTodos();

        var request = new { Limit = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.True(result.List.Count >= 3); // We created at least 3 todos
        Assert.True(result.List.Count <= 10); // Respects the limit
    }

    [Fact]
    public async Task GetTodoList_WithSmallLimit_ShouldRespectLimit()
    {
        // Arrange - Create multiple todos
        await CreateTestTodos();

        var request = new { Limit = 2 };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Equal(2, result.List.Count);
    }

    [Fact]
    public async Task GetTodoList_WithCursor_ShouldReturnPaginatedResults()
    {
        // Arrange - Create todos and get the first one's ID for cursor
        var todoIds = await CreateTestTodos();
        var cursorId = todoIds.First();

        var request = new { Limit = 10, Cursor = cursorId };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        // Should not include the cursor todo itself
        Assert.DoesNotContain(result.List, t => t.Id == cursorId);
    }

    [Fact]
    public async Task GetTodoList_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange - Clean database first
        await CleanDatabase();
        
        var request = new { Limit = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Empty(result.List);
    }

    [Fact]
    public async Task GetTodoList_WithZeroLimit_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateTestTodos();
        var request = new { Limit = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.Empty(result.List);
    }

    [Fact]
    public async Task GetTodoList_ShouldReturnTodosInCorrectOrder()
    {
        // Arrange - Create todos with different creation times
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var todos = new[]
            {
                new Todo
                {
                    Id = Guid.NewGuid(), Title = "First Todo", Description = "First", IsCompleted = false,
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-2)
                },
                new Todo
                {
                    Id = Guid.NewGuid(), Title = "Second Todo", Description = "Second", IsCompleted = false,
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
                },
                new Todo
                {
                    Id = Guid.NewGuid(), Title = "Third Todo", Description = "Third", IsCompleted = false,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            };
            context.Todos.AddRange(todos);
            await context.SaveChangesAsync();
        }

        var request = new { Limit = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/get-todo-list", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TodoListResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(result);
        Assert.True(result.List.Count >= 3);

        // Verify ordering (should be by ID, not CreatedAt)
        for (int i = 0; i < result.List.Count - 1; i++)
        {
            Assert.True(result.List[i].Id.CompareTo(result.List[i + 1].Id) <= 0);
        }
    }

    private async Task<List<Guid>> CreateTestTodos()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();

        var todos = new[]
        {
            new Todo
            {
                Id = Guid.NewGuid(), Title = "Todo 1", Description = "Description 1", IsCompleted = false,
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-10)
            },
            new Todo
            {
                Id = Guid.NewGuid(), Title = "Todo 2", Description = "Description 2", IsCompleted = true,
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5)
            },
            new Todo
            {
                Id = Guid.NewGuid(), Title = "Todo 3", Description = "Description 3", IsCompleted = false,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        context.Todos.AddRange(todos);
        await context.SaveChangesAsync();

        return todos.Select(t => t.Id).ToList();
    }

    private async Task CleanDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();

        // Delete all todos
        context.Todos.RemoveRange(context.Todos);
        await context.SaveChangesAsync();
    }

    private record TodoListResponse(List<TodoItem> List, Guid? NextCursor, bool HasMore);

    private record TodoItem(
        Guid Id,
        string Title,
        bool IsCompleted,
        DateTimeOffset CreatedAt
    );
}