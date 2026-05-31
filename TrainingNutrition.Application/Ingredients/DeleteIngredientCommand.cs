
using MediatR;

namespace TrainingNutrition.Application.Ingredients;

public sealed record DeleteIngredientCommand (
    Guid Id
) : IRequest;