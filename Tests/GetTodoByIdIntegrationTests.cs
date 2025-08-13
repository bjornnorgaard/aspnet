using System.Net;
using System.Text.Json;
using Application.Database;
using Application.Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests;

public class GetTodoByIdIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GetTodoByIdIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTodoById_WithValidId_ShouldReturnTodo()
    {
        // Arrange - Create a todo first
        var todoId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var todo = new Todo
            {
                Id = todoId,
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync($"/v1/todos/get-todo-by-id/{todoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(createdTodo);
        Assert.Equal(todoId, createdTodo.Id);
        Assert.Equal("Test Todo", createdTodo.Title);
        Assert.Equal("Test Description", createdTodo.Description);
        Assert.False(createdTodo.IsCompleted);
    }

    [Fact]
    public async Task GetTodoById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/v1/todos/get-todo-by-id/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_WithCompletedTodo_ShouldReturnCompletedTodo()
    {
        // Arrange - Create a completed todo
        var todoId = Guid.NewGuid();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var todo = new Todo
            {
                Id = todoId,
                Title = "Completed Todo",
                Description = "This todo is completed",
                IsCompleted = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
            };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync($"/v1/todos/get-todo-by-id/{todoId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(createdTodo);
        Assert.Equal(todoId, createdTodo.Id);
        Assert.True(createdTodo.IsCompleted);
    }

    private record TodoResponse(
        Guid Id,
        string Title,
        string Description,
        bool IsCompleted,
        DateTimeOffset CreatedAt
    );
}