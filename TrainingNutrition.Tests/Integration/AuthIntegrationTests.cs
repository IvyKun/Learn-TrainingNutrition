
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Infrastructure;

namespace TrainingNutrition.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
     private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    
    public async Task InitializeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Set<AppUser>().RemoveRange(db.Set<AppUser>());
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RegisterUser_ValidData_ReturnsUserId()
    {
        // Arrange
        RegisterRequest request = new(
            Email: "test@test.com",
            Password: "Password1234.");

       // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Guid id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task LoginUser_ValidData_ReturnsUserToken()
    {
        // Arrange

        RegisterRequest registerRequest = new(
            Email: "test@test.com",
            Password: "Password1234.");

        HttpResponseMessage registerResponse = await _httpClient.PostAsJsonAsync("/auth/register", registerRequest);

        LoginRequest request = new(
            Email: "test@test.com",
            Password: "Password1234.");

       // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        string? token = await response.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_Returns401()
    {
        // Arrange

        RegisterRequest registerRequest = new(
            Email: "test@test.com",
            Password: "Password1234.");

        HttpResponseMessage registerResponse = await _httpClient.PostAsJsonAsync("/auth/register", registerRequest);

        LoginRequest request = new(
            Email: "test@test.com",
            Password: "BadPassword");

       // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }

    [Fact]
    public async Task LoginUser_InvalidEmail_Returns401()
    {
        // Arrange

        LoginRequest request = new(
            Email: "invalid@test.com",
            Password: "Password1234.");

       // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }

    [Fact]
    public async Task LoginUser_InvalidEmailFormat_Returns400()
    {
        // Arrange

        LoginRequest request = new(
            Email: "invalid.test.com",
            Password: "Password1234.");

       // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }

}