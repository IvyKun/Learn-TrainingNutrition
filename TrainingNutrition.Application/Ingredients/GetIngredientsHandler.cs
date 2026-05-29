using MediatR;
using TrainingNutrition.Application.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;

namespace TrainingNutrition.Application.Ingredients;

public class GetIngredientsHandler : IRequestHandler<GetIngredientsQuery, IReadOnlyList<IngredientResponse>>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientsHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IReadOnlyList<IngredientResponse>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetAllAsync(request.SearchTerm, cancellationToken);

        var ingredientResponseList = new List<IngredientResponse>();

        foreach(var ingredient in ingredients)
        {
            var ingredientResponse = new IngredientResponse(
                ingredient.Id,
                ingredient.Name,
                ingredient.Brand,
                ingredient.MacrosPer100g.Protein,
                ingredient.MacrosPer100g.Carbs,
                ingredient.MacrosPer100g.Fat,
                ingredient.MacrosPer100g.Fiber,
                ingredient.MacrosPer100g.Salt,
                ingredient.MacrosPer100g.Calories
            );

            ingredientResponseList.Add(ingredientResponse);
        }

        return ingredientResponseList;
    }
}