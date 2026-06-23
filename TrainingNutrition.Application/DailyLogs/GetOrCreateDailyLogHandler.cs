using MediatR;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Application.DailyLogs;

public sealed class GetOrCreateDailyLogHandler : IRequestHandler<GetOrCreateDailyLogCommand, DailyLogResponse>
{
     private readonly IDailyLogRepository _dailyLogRepository;

    public GetOrCreateDailyLogHandler(IDailyLogRepository dailyLogRepository)
    {
        _dailyLogRepository = dailyLogRepository;
    }

    public async Task<DailyLogResponse> Handle(GetOrCreateDailyLogCommand request, CancellationToken cancellationToken)
    {
        var dailyLog = await _dailyLogRepository.GetByDateAsync(request.UserId, request.Date, cancellationToken);

        if(dailyLog == null)
        {
            dailyLog = new DailyLog(request.UserId, request.Date);

            await _dailyLogRepository.AddAsync(dailyLog, cancellationToken);
        }

        return new DailyLogResponse(dailyLog.Date, dailyLog.GetTotalCalories());
    }
}