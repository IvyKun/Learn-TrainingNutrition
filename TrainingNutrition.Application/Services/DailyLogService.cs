using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Application.Services;

public sealed class DailyLogService
{
    private readonly IDailyLogRepository _repository;

    public DailyLogService(IDailyLogRepository respository)
    {
        if(respository is null)
        {
            throw new ArgumentNullException(nameof(respository));
        }

        _repository = respository;
    }

    public async Task<DailyLog> GetOrCreateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByDateAsync(date, cancellationToken);

        if(existing is not null)
        {
            return existing;
        }

        var newLog = new DailyLog(date);

        await _repository.SaveAsync(newLog, cancellationToken);

        return newLog;
        
    }
}
