using MediatR;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Application.Ingredients;

public class CreateIngredientHandler : IRequestHandler<CreateIngredientCommand, Guid>
{
    private readonly IIngredientRepository _ingredientRepository;

    public CreateIngredientHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<Guid> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var macros = new Macronutrients(request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt);
        var ingredient = new Ingredient(request.Name, macros);

        await _ingredientRepository.AddAsync(ingredient, cancellationToken);

        return ingredient.Id;
    }
}