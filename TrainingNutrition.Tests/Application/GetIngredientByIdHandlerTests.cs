
using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Tests.Application;

public sealed class GetIngredientByIdHandlerTests
{

    [Fact]
    public async Task Handle_IngredientFound()
    {

       // Arrange
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        
        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ingredient);

        var handler = new GetIngredientByIdHandler(mockRepo.Object);
        var query = new GetIngredientByIdQuery(ingredient.Id);

        // Act
        var ingredientResponse = await handler.Handle(query, CancellationToken.None);

        // Assert

        Assert.NotNull(ingredientResponse);

        Assert.Equal(ingredient.Id, ingredientResponse.Id);
        Assert.Equal(ingredient.Name, ingredientResponse.Name);
        Assert.Equal(ingredient.MacrosPer100g.Protein, ingredientResponse.Protein);
        Assert.Equal(ingredient.MacrosPer100g.Carbs, ingredientResponse.Carbs);
        Assert.Equal(ingredient.MacrosPer100g.Fat, ingredientResponse.Fat);
        Assert.Equal(ingredient.MacrosPer100g.Fiber, ingredientResponse.Fiber);
        Assert.Equal(ingredient.MacrosPer100g.Salt, ingredientResponse.Salt);
      
    }

    [Fact]
    public async Task Handle_IngredientNotFound()
    {

       // Arrange
        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient?)null);

        var handler = new GetIngredientByIdHandler(mockRepo.Object);
        var query = new GetIngredientByIdQuery(Guid.NewGuid());

        // Act
        var ingredientResponse = await handler.Handle(query, CancellationToken.None);

        // Assert

        Assert.Null(ingredientResponse);
      
    }
    
    
}
