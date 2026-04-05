namespace TrainingNutrition.Api.DTOs;

public sealed record CreateIngredientRequest(
    string Name,
    decimal Protein,
    decimal Carbs,
    decimal Fat,
    decimal Fiber,
    decimal Salt
);