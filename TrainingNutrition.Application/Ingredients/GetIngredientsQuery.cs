using MediatR;

namespace TrainingNutrition.Application.Ingredients;

public sealed record GetIngredientsQuery(string? SearchTerm) : IRequest<IReadOnlyList<IngredientResponse>>;