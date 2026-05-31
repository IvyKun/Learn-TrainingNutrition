using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Application.Ingredients;

public class DeleteIngredientHandler : IRequestHandler<DeleteIngredientCommand>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly HybridCache _cache;

    public DeleteIngredientHandler(IIngredientRepository ingredientRepository, HybridCache cache)
    {
        _ingredientRepository = ingredientRepository;
        _cache = cache;
    }

    public async Task Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.Id, cancellationToken);

        if(ingredient == null)
        {
            throw new NotFoundException("Ingredient not found");
        }

        await _ingredientRepository.DeleteAsync(ingredient.Id, cancellationToken);

        await _cache.RemoveAsync($"ingredient-{request.Id}", cancellationToken);

    }

}