using Microsoft.EntityFrameworkCore;
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

    public async Task<IReadOnlyList<Ingredient>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return  await _db.Ingredients.ToListAsync(cancellationToken);
        }

        return await _db.Ingredients.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || (x.Brand != null && x.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))).ToListAsync(cancellationToken);
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Ingredients.FindAsync([id], cancellationToken);
    }
}