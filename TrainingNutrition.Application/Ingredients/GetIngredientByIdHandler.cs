using MediatR;
using TrainingNutrition.Application.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;

namespace TrainingNutrition.Application.Ingredients;

public class GetIngredientByIdHandler : IRequestHandler<GetIngredientByIdQuery, IngredientResponse?>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly HybridCache _cache;

    public GetIngredientByIdHandler(IIngredientRepository ingredientRepository, HybridCache cache)
    {
        _ingredientRepository = ingredientRepository;
        _cache = cache;
    }

    public async Task<IngredientResponse?> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"ingredient-{request.Id}",
            async ct =>
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(request.Id, ct);

                if (ingredient is null)
                    return null;

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
            },
            cancellationToken: cancellationToken
        );
        
        
    }
}