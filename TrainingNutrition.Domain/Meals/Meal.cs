using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Dishes;

namespace TrainingNutrition.Domain.Meals;

public sealed class Meal
{
    private readonly List<Dish> _dishes = new();

    public MealType Type { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }

    public IReadOnlyList<Dish> Dishes => _dishes;

    public Meal(MealType type, string name, DateTimeOffset occurredAt)
    {
        Type = type;
        Name = ValidateName(name);
        OccurredAt = occurredAt;
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangeType(MealType type)
    {
        Type = type;
    }

    public void ChangeOccurredAt(DateTimeOffset occurredAt)
    {
        OccurredAt = occurredAt;
    }

    public void AddDish(Dish dish)
    {
        if (dish is null)
            throw new ArgumentNullException(nameof(dish));

        _dishes.Add(dish);
    }

    public int RemoveDish(Dish dish)
    {
        if (dish is null)
            throw new ArgumentNullException(nameof(dish));

        return _dishes.RemoveAll(d => ReferenceEquals(d, dish));
    }

    public Macronutrients GetTotalMacros()
    {
        decimal protein = 0m;
        decimal carbs = 0m;
        decimal fat = 0m;

        foreach (var dish in _dishes)
        {
            var macros = dish.GetTotalMacros();
            protein += macros.Protein;
            carbs += macros.Carbs;
            fat += macros.Fat;
        }

        return new Macronutrients(protein, carbs, fat);
    }

    public int GetTotalCalories() => GetTotalMacros().Calories;

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Meal name cannot be empty.", nameof(name));

        name = name.Trim();

        if (name.Length > 100)
            throw new ArgumentException("Meal name cannot be longer than 100 characters.", nameof(name));

        return name;
    }
}
