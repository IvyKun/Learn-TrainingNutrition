using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Application.Abstractions;

public interface IIngredientRepository
{
     Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
}