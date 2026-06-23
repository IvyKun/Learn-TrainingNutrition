using MediatR;

namespace TrainingNutrition.Application.DailyLogs;

public sealed record AddIngredientEntryCommand(
    Guid UserId,
    DateOnly Date,
    Guid MealId,
    Guid IngredientId,
    int Grams
) : IRequest;