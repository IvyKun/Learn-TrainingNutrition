using MediatR;

namespace TrainingNutrition.Application.Auth;

public sealed record LoginCommand(
    string Email, 
    string Password
) : IRequest<string>;