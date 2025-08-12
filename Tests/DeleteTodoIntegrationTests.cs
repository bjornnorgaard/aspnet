using System.Net;
using Api.Database;
using Api.Database.Models;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests;

public class DeleteTodoIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DeleteTodoIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task DeleteTodo_WithValidId_ShouldDeleteTodoAndReturnNoContent()
    {
        // Arrange - Create a todo first
        var todoId = await CreateTestTodo("Todo to Delete", "This todo will be deleted");

        // Act
        var response = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the todo was deleted from the database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var deletedTodo = await context.Todos.FindAsync(todoId);

        Assert.Null(deletedTodo);
    }

    [Fact]
    public async Task DeleteTodo_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/v1/todos/delete-todo/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains($"Todo with id {nonExistentId} not found", responseContent);
    }

    [Fact]
    public async Task DeleteTodo_CompletedTodo_ShouldDeleteSuccessfully()
    {
        // Arrange - Create a completed todo
        var todoId = await CreateTestTodo("Completed Todo", "This completed todo will be deleted", true);

        // Act
        var response = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the todo was deleted from the database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var deletedTodo = await context.Todos.FindAsync(todoId);

        Assert.Null(deletedTodo);
    }

    [Fact]
    public async Task DeleteTodo_MultipleTodos_ShouldDeleteOnlySpecifiedTodo()
    {
        // Arrange - Create multiple todos
        var todoId1 = await CreateTestTodo("Todo 1", "First todo");
        var todoId2 = await CreateTestTodo("Todo 2", "Second todo");
        var todoId3 = await CreateTestTodo("Todo 3", "Third todo");

        // Act - Delete only the second todo
        var response = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoId2}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify only the specified todo was deleted
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();

        var todo1 = await context.Todos.FindAsync(todoId1);
        var todo2 = await context.Todos.FindAsync(todoId2);
        var todo3 = await context.Todos.FindAsync(todoId3);

        Assert.NotNull(todo1); // Should still exist
        Assert.Null(todo2); // Should be deleted
        Assert.NotNull(todo3); // Should still exist
    }

    [Fact]
    public async Task DeleteTodo_TwiceWithSameId_ShouldReturnNotFoundOnSecondCall()
    {
        // Arrange - Create a todo
        var todoId = await CreateTestTodo("Todo to Delete Twice", "This todo will be deleted twice");

        // Act - Delete the todo first time
        var firstResponse = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoId}");

        // Assert first deletion
        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);

        // Act - Try to delete the same todo again
        var secondResponse = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoId}");

        // Assert second deletion should fail
        Assert.Equal(HttpStatusCode.NotFound, secondResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_DoesNotAffectOtherTodosCount()
    {
        // Arrange - Create multiple todos
        var initialTodos = new[]
        {
            await CreateTestTodo("Todo 1", "First todo"),
            await CreateTestTodo("Todo 2", "Second todo"),
            await CreateTestTodo("Todo 3", "Third todo"),
            await CreateTestTodo("Todo 4", "Fourth todo")
        };

        // Get initial count
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var initialCount = context.Todos.Count();
            Assert.True(initialCount >= 4);
        }

        // Act - Delete one todo
        var todoToDelete = initialTodos[1];
        var response = await _client.DeleteAsync($"/v1/todos/delete-todo/{todoToDelete}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify count decreased by exactly one
        using var newScope = _factory.Services.CreateScope();
        var newContext = newScope.ServiceProvider.GetRequiredService<Context>();
        var remainingTodos = newContext.Todos.Where(t => initialTodos.Contains(t.Id)).ToList();
        Assert.Equal(3, remainingTodos.Count);

        // Verify the correct todo was deleted
        Assert.DoesNotContain(remainingTodos, t => t.Id == todoToDelete);
    }

    private async Task<Guid> CreateTestTodo(string title, string description, bool isCompleted = false)
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
}