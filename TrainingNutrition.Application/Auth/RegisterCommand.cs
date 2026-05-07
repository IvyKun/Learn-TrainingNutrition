using MediatR;
using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Application.Auth;

public sealed record RegisterCommand(
    string Email, 
    string Password
) : IRequest<Guid>;