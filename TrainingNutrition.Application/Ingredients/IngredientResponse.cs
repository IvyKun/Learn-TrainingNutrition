
namespace TrainingNutrition.Application.Ingredients;

public sealed record IngredientResponse(
    Guid Id,
    string Name,
    decimal Protein,
    decimal Carbs,
    decimal Fat,
    decimal Fiber,
    decimal Salt,
    int CaloriesPer100g
);