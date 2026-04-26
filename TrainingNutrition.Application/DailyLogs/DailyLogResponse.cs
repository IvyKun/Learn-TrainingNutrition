namespace TrainingNutrition.Application.DailyLogs;

public sealed record DailyLogResponse(
    DateOnly Date, 
    int TotalCalories
);