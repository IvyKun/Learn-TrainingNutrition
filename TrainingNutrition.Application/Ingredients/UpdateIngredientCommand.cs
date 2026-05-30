
using MediatR;

namespace TrainingNutrition.Application.Ingredients;

public sealed record UpdateIngredientCommand(
    Guid Id,
    string Name,
    decimal Protein,
    decimal Carbs,
    decimal Fat,
    decimal Fiber,
    decimal Salt,
    string? Brand
) : IRequest;