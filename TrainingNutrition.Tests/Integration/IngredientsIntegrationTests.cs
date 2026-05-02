
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Infrastructure;

namespace TrainingNutrition.Tests.Integration;

public class IngredientIntregrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
     private readonly CustomWebApplicationFactory _factory;

    public IngredientIntregrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    
    public async Task InitializeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Ingredients.RemoveRange(db.Ingredients);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task PostIngredient_ValidRequest_Returns200WithId()
    {
        // Arrange
        CreateIngredientRequest request = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m);

        // Act
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/ingredients", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Guid id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task GetIngredient_ExistingId_Returns200WithData()
    {
        // Arrange — create one first
        CreateIngredientRequest request = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m);

        HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/ingredients", request);
        Guid id = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/ingredients/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IngredientResponse? ingredient = await response.Content.ReadFromJsonAsync<IngredientResponse>();
        Assert.NotNull(ingredient);
        Assert.Equal("Chicken Breast", ingredient.Name);
    }

    [Fact]
    public async Task GetIngredient_NonExistingId_Returns404()
    {
        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/ingredients/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}