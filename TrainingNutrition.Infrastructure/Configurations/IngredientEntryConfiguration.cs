using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Dishes;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Infrastructure.Configurations;

public class IngredientEntryConfiguration : IEntityTypeConfiguration<IngredientEntry>
{
    public void Configure(EntityTypeBuilder<IngredientEntry> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Grams, grams =>
        {
            grams.Property(g => g.Value)
                .HasColumnName("Grams")
                .IsRequired();
        });

        builder.Ignore(x => x.Macros);
        builder.Ignore(x => x.Calories);

        // Relationship: each IngredientEntry references one Ingredient
        builder.HasOne(x => x.Ingredient)
            .WithMany()
            .HasForeignKey("IngredientId")
            .IsRequired();


    }
}