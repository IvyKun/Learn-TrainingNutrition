using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Meals;

namespace TrainingNutrition.Domain.Tracking;

public sealed class DailyLog
{
    public Guid Id { get; private init; } = Guid.NewGuid();

    public Guid UserId { get; private init; }

    private readonly List<Meal> _meals = new();

    public DateOnly Date { get; }
    public IReadOnlyList<Meal> Meals => _meals;

    private DailyLog() { } // For EF Core only
     
    public DailyLog(Guid userId, DateOnly date)
    {
        UserId = userId;
        Date = date;
    }

    public void AddMeal(Meal meal)
    {
        if(meal is null)
        {
            throw new ArgumentNullException(nameof(meal));
        }
        
        _meals.Add(meal);
    }
    
    public int RemoveMeal(Meal meal)
    {
        if(meal is null)
        {
            throw new ArgumentNullException(nameof(meal));
        }

        return _meals.RemoveAll(m => ReferenceEquals(m, meal));
    }

    public Macronutrients GetTotalMacros()
    {
        decimal protein = 0m;
        decimal carbs = 0m;
        decimal fat = 0m;
        decimal fiber = 0m;
        decimal salt = 0m;

        foreach (var meal in _meals)
        {
            var macros = meal.GetTotalMacros();
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