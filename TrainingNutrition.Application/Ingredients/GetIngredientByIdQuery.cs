using MediatR;

namespace TrainingNutrition.Application.Ingredients;

public sealed record GetIngredientByIdQuery(Guid Id) : IRequest<IngredientResponse?>;