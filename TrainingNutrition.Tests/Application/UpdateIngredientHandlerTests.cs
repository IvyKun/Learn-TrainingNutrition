
using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Tests.Common;

namespace TrainingNutrition.Tests.Application;

public sealed class UpdateIngredientHandlerTests
{
        [Fact]
        public async Task Handle_ShouldCallUpdateAsync_WhenIngredientExists()
        {
        // Arrange
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));

        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ingredient);

        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

        var handler = new UpdateIngredientHandler(mockRepo.Object, new NoOpHybridCache());
        var command = new UpdateIngredientCommand(ingredient.Id, "Updated Name", 25m, 5m, 10m, 2m, 0.5m, "BrandX");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert

        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Ingredient>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenIngredientNotFound()
        {
        // Arrange
        var mockRepo = new Mock<IIngredientRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Ingredient?)null);

        var handler = new UpdateIngredientHandler(mockRepo.Object, new NoOpHybridCache());
        var command = new UpdateIngredientCommand(Guid.NewGuid(), "Updated Name", 25m, 5m, 10m, 2m, 0.5m, null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));

        }
    

}