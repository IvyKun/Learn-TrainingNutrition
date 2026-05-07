
using Microsoft.AspNetCore.Identity;
using TrainingNutrition.Application.Abstractions;

namespace TrainingNutrition.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;

    public IdentityService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Guid> RegisterAsync(string email, string password)
    {
        AppUser appUser = new AppUser { Email = email, UserName = email };
        
        IdentityResult identityResult = await _userManager.CreateAsync(appUser, password);

        if (!identityResult.Succeeded)
        {
            string errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return Guid.Parse(appUser.Id);

    }
}