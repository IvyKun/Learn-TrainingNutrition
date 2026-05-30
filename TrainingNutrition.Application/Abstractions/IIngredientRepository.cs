using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Application.Abstractions;

public interface IIngredientRepository
{
     Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken = default);

     Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default);

     Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

     Task<IReadOnlyList<Ingredient>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default);
}