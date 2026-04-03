using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Domain.Dishes;

public sealed class Dish
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    
    private readonly List<IngredientEntry> _entries = new();

    public string Name { get; private set; }

    public IReadOnlyList<IngredientEntry> Entries => _entries;

    public Macronutrients GetTotalMacros()
    {
        decimal protein = 0m;
        decimal carbs = 0m;
        decimal fat = 0m;
        decimal fiber = 0m;
        decimal salt = 0m;

        foreach (var entry in _entries)
        {
            var macros = entry.Macros;
            protein += macros.Protein;
            carbs += macros.Carbs;
            fat += macros.Fat;
            fiber += macros.Fiber;
            salt += macros.Salt;
        }

        return new Macronutrients(protein, carbs, fat, fiber, salt);
    }

    public int GetTotalCalories() => GetTotalMacros().Calories;


    public Dish(string name)
    {
        Name = ValidateName(name);
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public void AddIngredient(IngredientEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));

        _entries.Add(entry);
    }

    public int RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient is null)
            throw new ArgumentNullException(nameof(ingredient));

        return _entries.RemoveAll(e => Equals(e.Ingredient, ingredient));
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Dish name cannot be empty.", nameof(name));

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException("Dish name cannot be longer than 100 characters.", nameof(name));

        return name;
    }
}
