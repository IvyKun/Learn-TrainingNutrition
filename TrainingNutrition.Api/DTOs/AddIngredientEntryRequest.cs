namespace TrainingNutrition.Api.DTOs;

public sealed record AddIngredientEntryRequest(
    Guid IngredientId,
    int Grams
);