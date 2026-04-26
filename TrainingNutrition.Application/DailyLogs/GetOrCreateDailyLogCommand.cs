
using MediatR;

namespace TrainingNutrition.Application.DailyLogs;

public sealed record GetOrCreateDailyLogCommand
(
    Guid UserId,
    DateOnly Date

) : IRequest<DailyLogResponse>;