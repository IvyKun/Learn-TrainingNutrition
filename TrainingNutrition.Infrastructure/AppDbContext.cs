using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;
using TrainingNutrition.Domain.Tracking;
using TrainingNutrition.Domain.Users;

namespace TrainingNutrition.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<IngredientEntry> IngredientEntries { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<DailyLog> DailyLogs { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}


