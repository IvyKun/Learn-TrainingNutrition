using MediatR;
using TrainingNutrition.Application.Abstractions;

namespace TrainingNutrition.Application.Ingredients;

public class GetIngredientByIdHandler : IRequestHandler<GetIngredientByIdQuery, IngredientResponse?>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientByIdHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IngredientResponse?> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredient =  await _ingredientRepository.GetByIdAsync(request.Id, cancellationToken);

        if(ingredient == null)
        {
            return null;
        }
        
        return new IngredientResponse(
            ingredient.Id,
            ingredient.Name,
            ingredient.MacrosPer100g.Protein,
            ingredient.MacrosPer100g.Carbs,
            ingredient.MacrosPer100g.Fat,
            ingredient.MacrosPer100g.Fiber,
            ingredient.MacrosPer100g.Salt,
            ingredient.MacrosPer100g.Calories
            );
        
        
    }
}