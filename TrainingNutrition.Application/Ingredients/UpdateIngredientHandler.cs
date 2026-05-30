using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Application.Ingredients;

public class UpdateIngredientHandler : IRequestHandler<UpdateIngredientCommand>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly HybridCache _cache;

    public UpdateIngredientHandler(IIngredientRepository ingredientRepository, HybridCache cache)
    {
        _ingredientRepository = ingredientRepository;
        _cache = cache;
    }

    public async Task Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        var macros = new Macronutrients(request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt);
        var ingredient = await _ingredientRepository.GetByIdAsync(request.Id, cancellationToken);

        if(ingredient == null)
        {
            throw new NotFoundException("Ingredient not found");
        }

        ingredient.Update(request.Name, macros, request.Brand);

        await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);

        await _cache.RemoveAsync($"ingredient-{request.Id}", cancellationToken);

    }

}