
using Microsoft.EntityFrameworkCore;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Infrastructure.Repositories;

public class EfDailyLogRepository : IDailyLogRepository
{
    private readonly AppDbContext _db;

    public EfDailyLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async  Task AddAsync(DailyLog dailyLog, CancellationToken cancellationToken = default)
    {
         _db.DailyLogs.Add(dailyLog);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DailyLog dailyLog, CancellationToken cancellationToken = default)
    {
        // EF Core change tracker detects the mutation automatically after GetByDateAsync
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<DailyLog?> GetByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
    {
            return await _db.DailyLogs
                            .Include(x => x.Meals)
                                .ThenInclude(m => m.IngredientEntries)
                                    .ThenInclude(e => e.Ingredient)
                            .FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date, cancellationToken);
    }


}