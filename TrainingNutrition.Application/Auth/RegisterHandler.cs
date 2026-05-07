using MediatR;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Application.Auth;

public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;

    public RegisterHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request.Email, request.Password);
    }
}