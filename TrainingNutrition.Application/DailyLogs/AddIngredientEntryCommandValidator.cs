using FluentValidation;

namespace TrainingNutrition.Application.DailyLogs;

public sealed class AddIngredientEntryCommandValidator : AbstractValidator<AddIngredientEntryCommand>
{
    public AddIngredientEntryCommandValidator()
    {
        RuleFor(x => x.MealId).NotEmpty();
        RuleFor(x => x.IngredientId).NotEmpty();
        RuleFor(x => x.Grams).GreaterThan(0);
    }
}