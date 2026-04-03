using Microsoft.EntityFrameworkCore;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Ingredient> Ingredients { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}


