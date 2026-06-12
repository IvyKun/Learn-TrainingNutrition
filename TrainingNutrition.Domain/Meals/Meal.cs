using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Domain.Meals;

public sealed class Meal

{
    public Guid Id { get; private init; } = Guid.NewGuid();


    public MealType Type { get; private set; }

    private readonly List<IngredientEntry> _ingredientEntries = new();

    public IReadOnlyList<IngredientEntry> IngredientEntries => _ingredientEntries;

    public Meal(MealType type)
    {
        Type = type;
    }

    public void AddIngredientEntry(IngredientEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));

        _ingredientEntries.Add(entry);
    }

    public bool RemoveIngredientEntry(Guid entryId)
    {
        var entry = _ingredientEntries.FirstOrDefault(e => e.Id == entryId);

        if (entry is null)
            return false;

        _ingredientEntries.Remove(entry);
        return true;
    }

    public Macronutrients GetTotalMacros()
    {
        decimal protein = 0m;
        decimal carbs = 0m;
        decimal fat = 0m;
        decimal fiber = 0m;
        decimal salt = 0m;

        foreach (var entry in _ingredientEntries)
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


}
