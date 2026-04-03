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

        // Name column: required, max length from domain validation, unique
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.OccurredAt);

       // Relationship: a Meal has many Dish
        builder.HasMany(x => x.Dishes)
            .WithOne()
            .HasForeignKey("MealId")
            .IsRequired();

    }
}