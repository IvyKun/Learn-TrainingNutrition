namespace TrainingNutrition.Api.DTOs;

public sealed record UpdateIngredientRequest(
    string Name,
    decimal Protein,
    decimal Carbs,
    decimal Fat,
    decimal Fiber,
    decimal Salt,
    string? Brand
);