
using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Tests.Application;

public sealed class CreateIngredientHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnIngredientId_WhenCommandIsValid()
    {

       // Arrange
        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var handler = new CreateIngredientHandler(mockRepo.Object);
        var command = new CreateIngredientCommand("Tuna", 21.0m, 1.0m, 1.0m, 0.0m, 1.1m);

        // Act
        var ingredientId = await handler.Handle(command, CancellationToken.None);

        // Assert

         Assert.NotEqual(ingredientId, Guid.Empty);
      
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce_WhenCommandIsValid()
    {

       // Arrange
        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        var handler = new CreateIngredientHandler(mockRepo.Object);
        var command = new CreateIngredientCommand("Tuna", 21.0m, 1.0m, 1.0m, 0.0m, 1.1m);

        // Act
        var ingredientId = await handler.Handle(command, CancellationToken.None);

        // Assert

         mockRepo.Verify(r => r.AddAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Once);
      
    }

}