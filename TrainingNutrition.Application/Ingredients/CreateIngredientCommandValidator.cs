using FluentValidation;

namespace TrainingNutrition.Application.Ingredients;

public sealed class CreateIngredientCommandValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Protein).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fat).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fiber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Salt).GreaterThanOrEqualTo(0);
    }
    
}