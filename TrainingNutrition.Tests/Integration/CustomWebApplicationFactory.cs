
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrainingNutrition.Infrastructure;
using TrainingNutrition.Tests.Common;

namespace TrainingNutrition.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing AppDbContext registration
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            // Register AppDbContext pointing to the test database
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql("Host=localhost;Port=5432;Database=trainingnutrition_test;Username=tnuser;Password=tnpassword"));

            // Replace HybridCache with NoOp to avoid Redis dependency in tests
            ServiceDescriptor? hybridCacheDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(HybridCache));
            if (hybridCacheDescriptor is not null)
                services.Remove(hybridCacheDescriptor);
            services.AddSingleton<HybridCache, NoOpHybridCache>();
        });

        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                options.TokenValidationParameters.ValidateIssuer = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidateLifetime = false;
                options.TokenValidationParameters.RequireSignedTokens = false;
            });
        });


        builder.UseEnvironment("Development");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        IHost host = base.CreateHost(builder);

        using IServiceScope scope = host.Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();

        return host;
    }

    public HttpClient CreateAuthenticatedClient(string userId = "00000000-0000-0000-0000-000000000001")
    {
        HttpClient client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateFakeToken(userId));
        return client;
    }

    public HttpClient CreateUnauthenticatedClient() => CreateClient();

    private static string CreateFakeToken(string userId)
    {
        JwtSecurityToken token = new(
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, "test@test.com")
            ],
            expires: DateTime.UtcNow.AddHours(1)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}