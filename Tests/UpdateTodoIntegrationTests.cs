using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Database;
using Application.Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests;

public class UpdateTodoIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UpdateTodoIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ShouldUpdateTodoAndReturnNoContent()
    {
        // Arrange - Create a todo first
        var todoId = await CreateTestTodo("Original Title", "Original Description", false);

        var request = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the todo was updated in the database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var updatedTodo = await context.Todos.FindAsync(todoId);

        Assert.NotNull(updatedTodo);
        Assert.Equal("Updated Title", updatedTodo.Title);
        Assert.Equal("Updated Description", updatedTodo.Description);
        Assert.True(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_MarkAsCompleted_ShouldUpdateCompletionStatus()
    {
        // Arrange - Create an incomplete todo
        var todoId = await CreateTestTodo("Test Todo", "Test Description", false);

        var request = new
        {
            Title = "Test Todo",
            Description = "Test Description",
            IsCompleted = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify completion status was updated
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var updatedTodo = await context.Todos.FindAsync(todoId);

        Assert.NotNull(updatedTodo);
        Assert.True(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_MarkAsIncomplete_ShouldUpdateCompletionStatus()
    {
        // Arrange - Create a completed todo
        var todoId = await CreateTestTodo("Completed Todo", "Completed Description", true);

        var request = new
        {
            Title = "Completed Todo",
            Description = "Completed Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify completion status was updated
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var updatedTodo = await context.Todos.FindAsync(todoId);

        Assert.NotNull(updatedTodo);
        Assert.False(updatedTodo.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{nonExistentId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains($"Todo with id {nonExistentId} not found", responseContent);
    }

    [Fact]
    public async Task UpdateTodo_WithEmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var todoId = await CreateTestTodo("Test Todo", "Test Description", false);

        var request = new
        {
            Title = "",
            Description = "Valid Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Contains("Invalid request parameters", errorResponse.Message);
    }

    [Fact]
    public async Task UpdateTodo_WithEmptyDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var todoId = await CreateTestTodo("Test Todo", "Test Description", false);

        var request = new
        {
            Title = "Valid Title",
            Description = "",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Contains("Invalid request parameters", errorResponse.Message);
    }

    [Fact]
    public async Task UpdateTodo_OnlyTitle_ShouldUpdateOnlyTitle()
    {
        // Arrange
        var todoId = await CreateTestTodo("Original Title", "Original Description", false);

        var request = new
        {
            Title = "New Title Only",
            Description = "Original Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/todos/update-todo/{todoId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify only title was updated
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var updatedTodo = await context.Todos.FindAsync(todoId);

        Assert.NotNull(updatedTodo);
        Assert.Equal("New Title Only", updatedTodo.Title);
        Assert.Equal("Original Description", updatedTodo.Description);
        Assert.False(updatedTodo.IsCompleted);
    }

    private async Task<Guid> CreateTestTodo(string title, string description, bool isCompleted)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();

        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            IsCompleted = isCompleted,
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        return todo.Id;
    }

    private record ErrorResponse(
        int StatusCode,
        string Message,
        string Details,
        DateTime Timestamp
    );
}