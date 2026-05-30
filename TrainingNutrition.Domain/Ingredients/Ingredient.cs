using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Domain.Ingredients;

public sealed class Ingredient
{
    public Guid Id { get; private init; } = Guid.NewGuid();

    public string Name { get; private set; }

    public string? Brand { get; private set; }


    public Macronutrients MacrosPer100g  { get; private set; }


    public int CaloriesPer100g => MacrosPer100g.Calories;

    private Ingredient() { } // For EF Core only
    
    public Ingredient(string name, Macronutrients macrosPer100g, string? brand = null)
    {
        Name = ValidateName(name);
        Brand = ValidateBrand(brand);
        MacrosPer100g = macrosPer100g ?? throw new ArgumentNullException(nameof(macrosPer100g));
    }

    public void Update(string name, Macronutrients macrosPer100g, string? brand)
    {
        Name = ValidateName(name);
        Brand = ValidateBrand(brand);
        MacrosPer100g = macrosPer100g ?? throw new ArgumentNullException(nameof(macrosPer100g));
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name cannot be empty.", nameof(name));

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException("Ingredient name cannot be longer than 100 characters.", nameof(name));

        return name;
    }

    private static string? ValidateBrand(string? brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return null;
        }

        brand = brand.Trim();

        if (brand.Length > 100)
            throw new ArgumentException("Ingredient brand cannot be longer than 100 characters.", nameof(brand));

        return brand;
    }
}
