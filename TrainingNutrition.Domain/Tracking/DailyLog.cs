using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Meals;

namespace TrainingNutrition.Domain.Tracking;

public sealed class DailyLog
{
    private readonly List<Meal> _meals = new();

    public DateOnly Date { get; }
    public IReadOnlyList<Meal> Meals => _meals;

    public DailyLog(DateOnly date)
    {
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

        foreach (var meal in _meals)
        {
            var macros = meal.GetTotalMacros();
            protein += macros.Protein;
            carbs += macros.Carbs;
            fat += macros.Fat;
        }

        return new Macronutrients(protein, carbs, fat);
    }

    public int GetTotalCalories() => GetTotalMacros().Calories;
}