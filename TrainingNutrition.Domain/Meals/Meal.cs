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

    public void AddIngredient(IngredientEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));

        _ingredientEntries.Add(entry);
    }

    public int RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient is null)
            throw new ArgumentNullException(nameof(ingredient));

        return _ingredientEntries.RemoveAll(e => Equals(e.Ingredient, ingredient));
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
