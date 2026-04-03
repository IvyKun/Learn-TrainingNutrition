
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Dishes;

namespace TrainingNutrition.Infrastructure.Configurations;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
       // Primary key
        builder.HasKey(x => x.Id);

         // Name column: required, max length from domain validation, unique
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Relationship: a Dish has many IngredientEntries
        builder.HasMany(x => x.Entries)
            .WithOne()
            .HasForeignKey("DishId")
            .IsRequired();
    }
}