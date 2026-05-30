using FluentValidation;

namespace TrainingNutrition.Application.Ingredients;

public sealed class UpdateIngredientCommandValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).MaximumLength(100);

        RuleFor(x => x.Protein).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Carbs).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fat).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fiber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Salt).GreaterThanOrEqualTo(0);
    }
    
}