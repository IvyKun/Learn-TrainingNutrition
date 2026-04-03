using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Infrastructure.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // Name column: required, max length from domain validation, unique
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // MacrosPer100g is a Value Object — maps to columns in the same table (owned type)
        builder.OwnsOne(x => x.MacrosPer100g, macro =>
        {
            macro.Property(m => m.Protein)
                .HasColumnName("Protein")
                .IsRequired();

            macro.Property(m => m.Carbs)
                .HasColumnName("Carbs")
                .IsRequired();

            macro.Property(m => m.Fat)
                .HasColumnName("Fat")
                .IsRequired();

            macro.Property(m => m.Fiber)
                .HasColumnName("Fiber")
                .IsRequired();

            macro.Property(m => m.Salt)
                .HasColumnName("Salt")
                .IsRequired();
        });

        // CaloriesPer100g is computed — do not persist
        builder.Ignore(x => x.CaloriesPer100g);


    }
}