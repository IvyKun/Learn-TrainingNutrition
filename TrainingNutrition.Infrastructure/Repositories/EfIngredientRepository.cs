using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Infrastructure.Repositories;

public class EfIngredientRepository : IIngredientRepository
{
    private readonly AppDbContext _db;

    public EfIngredientRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        _db.Ingredients.Add(ingredient);

        await _db.SaveChangesAsync(cancellationToken);
    }
}