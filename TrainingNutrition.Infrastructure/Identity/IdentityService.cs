
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrainingNutrition.Application.Abstractions;

namespace TrainingNutrition.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public IdentityService(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
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

    public async Task<string> LoginAsync(string email, string password)
    {
        AppUser? user = await _userManager.FindByEmailAsync(email);
        if(user == null)
        {
            throw new InvalidOperationException("Invalid credentials");
        }
        
        bool passwordValid = await _userManager.CheckPasswordAsync(user, password);
         if(!passwordValid)
        {
            throw new InvalidOperationException("Invalid credentials");
        }
        
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.ExpiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}