
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

    public async Task<DailyLog?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
          return await _db.DailyLogs.FirstOrDefaultAsync(x => x.Date == date, cancellationToken);
    }
}