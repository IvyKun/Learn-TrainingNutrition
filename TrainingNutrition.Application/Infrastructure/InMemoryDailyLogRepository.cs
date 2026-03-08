using TrainingNutrition.Domain.Tracking;
using TrainingNutrition.Application.Abstractions;

namespace TrainingNutrition.Application.Infrastructure;

public sealed class InMemoryDailyLogRepository : IDailyLogRepository
{
    private readonly Dictionary<DateOnly, DailyLog> _storage = new();
    public Task<DailyLog?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(date, out var log);
        return Task.FromResult(log);
    }

    public Task SaveAsync(DailyLog dailyLog, CancellationToken cancellationToken = default)
    {
       if(dailyLog is null)
        {
            throw new ArgumentNullException(nameof(dailyLog));
        }

        _storage[dailyLog.Date] = dailyLog;
        return Task.CompletedTask;
    }
}