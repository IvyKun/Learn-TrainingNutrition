
using MediatR;

namespace TrainingNutrition.Application.Ingredients;

public sealed record CreateIngredientCommand(
    string Name,
    decimal Protein,
    decimal Carbs,
    decimal Fat,
    decimal Fiber,
    decimal Salt,
    string? Brand = null
) : IRequest<Guid>;