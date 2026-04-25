
using FluentValidation;
using MediatR;
using TrainingNutrition.Application.Behaviors;
using TrainingNutrition.Application.Ingredients;

namespace TrainingNutrition.Tests.Application;
public sealed class ValidationBehaviorTests
{
     [Fact]
    public async Task Handle_WithNoValidators_CallsNext ()
    {
        // Arrange
        var behavior = new ValidationBehavior<CreateIngredientCommand, Guid>([]);
        var command = new CreateIngredientCommand("Tuna", 21.0m, 1.0m, 1.0m, 0.0m, 1.1m);
        var expectedId = Guid.NewGuid();
        RequestHandlerDelegate<Guid> next = _ => Task.FromResult(expectedId);

        // Act
        Guid result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CallsNext ()
    {
        // Arrange
        var validator = new CreateIngredientCommandValidator();
        var behavior = new ValidationBehavior<CreateIngredientCommand, Guid>([ validator ]);
        var command = new CreateIngredientCommand("Tuna", 21.0m, 1.0m, 1.0m, 0.0m, 1.1m);
        var expectedId = Guid.NewGuid();
        RequestHandlerDelegate<Guid> next = _ => Task.FromResult(expectedId);

        // Act
        Guid result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }

    
    [Fact]
    public async Task Handle_WithInvalidCommand_ThrowsValidationException  ()
    {
        // Arrange
        var validator = new CreateIngredientCommandValidator();
        var behavior = new ValidationBehavior<CreateIngredientCommand, Guid>([ validator ]);
        var command = new CreateIngredientCommand("", 21.0m, 1.0m, 1.0m, 0.0m, 1.1m);

        RequestHandlerDelegate<Guid> next = _ => Task.FromResult(Guid.NewGuid());


        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(command, next, CancellationToken.None));
    }
    
}