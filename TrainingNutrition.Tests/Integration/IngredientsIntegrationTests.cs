
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
       _httpClient = factory.CreateAuthenticatedClient();
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

    // Create

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

    // Update

    [Fact]
    public async Task UpdateIngredient_ValidRequest_Returns204()
    {
        // Arrange
        CreateIngredientRequest createRequest = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m);

        HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/ingredients", createRequest);
        Guid id = await createResponse.Content.ReadFromJsonAsync<Guid>();

        UpdateIngredientRequest request = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m,
            Brand: "Mercadona");

        // Act
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/ingredients/{id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

       // Verify the update was applied
        HttpResponseMessage getResponse = await _httpClient.GetAsync($"/ingredients/{id}");
        IngredientResponse? ingredient = await getResponse.Content.ReadFromJsonAsync<IngredientResponse>();
        Assert.NotNull(ingredient);
        Assert.Equal("Mercadona", ingredient.Brand);
    }

    [Fact]
    public async Task UpdateIngredient_NonExistingId_Returns404()
    {
        // Arrange
        UpdateIngredientRequest request = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m,
            Brand: "Mercadona");

        // Act
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/ingredients/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    // Delete

    [Fact]
    public async Task DeleteIngredient_ValidRequest_Returns204()
    {
        // Arrange
        CreateIngredientRequest createRequest = new(
            Name: "Chicken Breast",
            Protein: 31,
            Carbs: 0,
            Fat: 3.6m,
            Fiber: 0,
            Salt: 0.07m);

        HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("/ingredients", createRequest);
        Guid id = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        HttpResponseMessage response = await _httpClient.DeleteAsync($"/ingredients/{id}");

       // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

       // Verify the delete worked
        HttpResponseMessage getResponse = await _httpClient.GetAsync($"/ingredients/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteIngredient_NonExistingId_Returns404()
    {
        // Arrange

        // Act
        HttpResponseMessage response = await _httpClient.DeleteAsync($"/ingredients/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    
    // Get

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



    [Fact]
    public async Task GetIngredients_NoSearch_ReturnsAll ()
    {   
         // Arrange

        CreateIngredientRequest request1 = new(
        Name: "Chicken Breast",
        Protein: 31,
        Carbs: 0,
        Fat: 3.6m,
        Fiber: 0,
        Salt: 0.07m);

        await _httpClient.PostAsJsonAsync("/ingredients", request1);

        CreateIngredientRequest request2 = new(
        Name: "Chicken Breast 2",
        Protein: 31,
        Carbs: 0,
        Fat: 3.6m,
        Fiber: 0,
        Salt: 0.07m);

        await _httpClient.PostAsJsonAsync("/ingredients", request2);

        string searchTerm = string.Empty;

         // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/ingredients?search={searchTerm}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IReadOnlyList<IngredientResponse>? ingredients = await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientResponse>>();
        Assert.NotNull(ingredients);
        Assert.NotEmpty(ingredients);
        Assert.Equal(2, ingredients.Count);
    }

    [Fact]
    public async Task GetIngredients_WithSearch_ReturnsFiltered  ()
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
        
        string searchTerm = "chicken";

         // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/ingredients?search={searchTerm}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        IReadOnlyList<IngredientResponse>? ingredients = await response.Content.ReadFromJsonAsync<IReadOnlyList<IngredientResponse>>();
        Assert.NotNull(ingredients);
        Assert.NotEmpty(ingredients);
        Assert.Equal("Chicken Breast", ingredients.FirstOrDefault()?.Name);
    }


    // Auth

    [Fact]
    public async Task CreateIngredient_WithoutToken_Returns401()
    {
        // Arrange
        HttpClient unauthenticatedClient = _factory.CreateUnauthenticatedClient();
        CreateIngredientRequest request = new("Chicken", 31, 0, 3.6m, 0, 0.1m);

        // Act
        HttpResponseMessage response = await unauthenticatedClient.PostAsJsonAsync("/ingredients", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredient_WithoutToken_Returns401()
    {
        // Arrange
        HttpClient unauthenticatedClient = _factory.CreateUnauthenticatedClient();

        // Act
        HttpResponseMessage response = await unauthenticatedClient.GetAsync($"/ingredients/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}