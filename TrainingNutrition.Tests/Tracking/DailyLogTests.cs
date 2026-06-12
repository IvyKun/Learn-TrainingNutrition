using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Tests.Tracking;

public sealed class DailyLogTests
{

    [Fact]
    public void Constructor_ShouldCreateOneMealPerMealType()
    {
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        Assert.Equal(Enum.GetValues<MealType>().Length, log.Meals.Count);

        foreach (var mealType in Enum.GetValues<MealType>())
        {
            Assert.Contains(log.Meals, m => m.Type == mealType);
        }
    }

    [Fact]
    public void GetTotalMacros_ShouldSumMealsMacros()
    {   
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        var breakfast = log.Meals.First(m => m.Type == MealType.Breakfast);
        var lunch = log.Meals.First(m => m.Type == MealType.Lunch);

        breakfast.AddIngredientEntry(CreateSimpleIngredientEntry("Ingredient 1", protein: 10m, carbs: 20m, fat: 5m, grams: 100)); // 165 kcal
        lunch.AddIngredientEntry(CreateSimpleIngredientEntry("Ingredient 2", protein: 5m, carbs: 0m, fat: 10m, grams: 100));    // 110 kcal

        var total = log.GetTotalMacros();

    Assert.Equal(new Macronutrients(15m, 20m, 15m, 0m, 0m), total);
    }

    [Fact]
    public void GetTotalCalories_ShouldUseMacrosCaloriesRounding()
    {
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        var breakfast = log.Meals.First(m => m.Type == MealType.Breakfast);

        // 10P/20C/5F per 100g, con 50g => 5P/10C/2.5F => 82.5 -> 83
        breakfast.AddIngredientEntry(CreateSimpleIngredientEntry("Ingredient", protein: 10m, carbs: 20m, fat: 5m, grams: 50));

        Assert.Equal(83, log.GetTotalCalories());
    }

    private static IngredientEntry CreateSimpleIngredientEntry(string name, decimal protein, decimal carbs, decimal fat, int grams)
    {
        var ingredient = new Ingredient(name, new Macronutrients(protein, carbs, fat, 0m, 0m));
        var ingredientEntry = new IngredientEntry(ingredient, new Grams(grams));

        return ingredientEntry;
    }
}