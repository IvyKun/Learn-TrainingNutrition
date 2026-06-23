using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Application.Abstractions;

public interface IDailyLogRepository
{
    Task<DailyLog?> GetByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);

    Task UpdateAsync(DailyLog dailyLog, CancellationToken cancellationToken = default);

    Task AddAsync(DailyLog dailyLog, CancellationToken cancellationToken = default);
 
}