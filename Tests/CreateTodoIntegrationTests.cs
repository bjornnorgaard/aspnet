using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Database;
using Microsoft.Extensions.DependencyInjection;
using Tests.Infrastructure;

namespace Tests;

public class CreateTodoIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CreateTodoIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodo_WithValidData_ShouldReturnCreatedAndSaveTodoToDatabase()
    {
        // Arrange
        var request = new
        {
            Title = "Test Todo",
            Description = "This is a test todo item"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/create-todo", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var todo = JsonSerializer.Deserialize<TodoResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(todo);
        Assert.Equal(request.Title, todo.Title);
        Assert.Equal(request.Description, todo.Description);
        Assert.False(todo.IsCompleted);
        Assert.NotEqual(Guid.Empty, todo.Id);

        // Verify the todo was saved to the database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();
        var savedTodo = await context.Todos.FindAsync(todo.Id);

        Assert.NotNull(savedTodo);
        Assert.Equal(request.Title, savedTodo.Title);
        Assert.Equal(request.Description, savedTodo.Description);
        Assert.False(savedTodo.IsCompleted);
    }

    [Fact]
    public async Task CreateTodo_WithEmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            Title = "",
            Description = "Valid description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/create-todo", request);

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
    public async Task CreateTodo_WithEmptyDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            Title = "Valid title",
            Description = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/create-todo", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithNullTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new
        {
            Title = (string?)null,
            Description = "Valid description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/create-todo", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithLongTitle_ShouldCreateSuccessfully()
    {
        // Arrange
        var longTitle = new string('A', 90); // 90 characters - within the 100 char limit
        var request = new
        {
            Title = longTitle,
            Description = "Test description for long title"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/todos/create-todo", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var todo = JsonSerializer.Deserialize<TodoResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(todo);
        Assert.Equal(longTitle, todo.Title);
    }

    private record TodoResponse(
        Guid Id,
        string Title,
        string Description,
        bool IsCompleted,
        DateTimeOffset CreatedAt
    );

    private record ErrorResponse(
        int StatusCode,
        string Message,
        string Details,
        DateTime Timestamp
    );
}