using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Application.Abstractions;

public interface IIdentityService
{
    Task<Guid> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}