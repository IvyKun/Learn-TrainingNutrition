using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Meals;

namespace TrainingNutrition.Infrastructure.Configurations;

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
         // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

       // Relationship: a Meal has many IngredientEntries
        builder.HasMany(x => x.IngredientEntries)
            .WithOne()
            .HasForeignKey("MealId")
            .IsRequired();

    }
}