using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Domain.Ingredients;

public sealed record Ingredient
{
    public string Name { get; }
    public Macronutrients MacrosPer100g { get; }

    public int CaloriesPer100g => MacrosPer100g.Calories;

    public Ingredient(string name, Macronutrients macrosPer100g)
    {
        Name = ValidateName(name);
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
}
