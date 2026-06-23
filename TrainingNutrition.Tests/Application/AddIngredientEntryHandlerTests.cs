using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.DailyLogs;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Tests.Application;

public sealed class AddIngredientEntryHandlerTests
{

    [Fact]
    public async Task Handle_ShouldAddIngredientEntry_WhenAllDataIsOk()
    {
        // Arrange

        // Daily log
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(dailyLog.UserId, dailyLog.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyLog);

        // Ingredient
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        
        var mockRepoIngredient = new Mock<IIngredientRepository>();
        mockRepoIngredient.Setup(r => r.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ingredient);

        // Igredient Entry
        var breakfast = dailyLog.Meals.First(m => m.Type == MealType.Breakfast);

        var handler = new AddIngredientEntryHandler(mockRepo.Object, mockRepoIngredient.Object);
        var command = new AddIngredientEntryCommand(dailyLog.UserId, dailyLog.Date, breakfast.Id, ingredient.Id, 50);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, breakfast.IngredientEntries.Count);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Once);
      
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenDailyLogNotFound()
    {
        // Arrange

        // Daily log
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(dailyLog.UserId, dailyLog.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DailyLog?)null);

        // Ingredient
        var mockRepoIngredient = new Mock<IIngredientRepository>();

        // Igredient Entry

        var handler = new AddIngredientEntryHandler(mockRepo.Object, mockRepoIngredient.Object);
        var command = new AddIngredientEntryCommand(dailyLog.UserId, dailyLog.Date, Guid.NewGuid(), Guid.NewGuid(), 50);


        // Act & Assert

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenMealNotFound()
    {
       // Arrange

        // Daily log
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(dailyLog.UserId, dailyLog.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyLog);

         // Ingredient
        var mockRepoIngredient = new Mock<IIngredientRepository>();

        // Igredient Entry
        var handler = new AddIngredientEntryHandler(mockRepo.Object, mockRepoIngredient.Object);
        var command = new AddIngredientEntryCommand(dailyLog.UserId, dailyLog.Date, Guid.NewGuid(), Guid.NewGuid(), 50);


        // Act & Assert

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenIngredientNotFound()
    {
       // Arrange

        // Daily log
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(dailyLog.UserId, dailyLog.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyLog);

        // Ingredient
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        
        var mockRepoIngredient = new Mock<IIngredientRepository>();
        mockRepoIngredient.Setup(r => r.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ingredient?)null);

        // Igredient Entry
        var breakfast = dailyLog.Meals.First(m => m.Type == MealType.Breakfast);

        var handler = new AddIngredientEntryHandler(mockRepo.Object, mockRepoIngredient.Object);
        var command = new AddIngredientEntryCommand(dailyLog.UserId, dailyLog.Date, breakfast.Id, ingredient.Id, 50);


        // Act & Assert

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Never);

    }


}