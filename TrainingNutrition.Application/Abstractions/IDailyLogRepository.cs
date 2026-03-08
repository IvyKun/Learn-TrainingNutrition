using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Application.Abstractions;

public interface IDailyLogRepository
{
    Task<DailyLog?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task SaveAsync(DailyLog dailyLog, CancellationToken cancellationToken = default);
}