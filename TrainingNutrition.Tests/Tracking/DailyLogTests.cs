using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Dishes;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Tests.Tracking;

public sealed class DailyLogTests
{

    [Fact]
    public void AddMeal_ShouldThrow_WhenMealIsNull()
    {
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        Assert.Throws<ArgumentNullException>(() =>
        {
            log.AddMeal(null!);
        });
    }

    [Fact]
    public void RemoveMeal_ShouldThrow_WhenMealIsNull()
    {   
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        Assert.Throws<ArgumentNullException>(() =>
        {
            log.RemoveMeal(null!);
        });
    }

    [Fact]
    public void RemoveMeal_ShouldRemoveOnlyMatchingReference_AndReturnCountRemoved()
    {   
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        var mealA = new Meal(MealType.Meal, "Breakfast", DateTimeOffset.UtcNow);
        var mealB = new Meal(MealType.Snack, "Snack", DateTimeOffset.UtcNow);

        log.AddMeal(mealA);
        log.AddMeal(mealA); // same reference twice
        log.AddMeal(mealB);

        var removed = log.RemoveMeal(mealA);

        Assert.Equal(2, removed);
        Assert.Single(log.Meals);
        Assert.Same(mealB, log.Meals[0]);
    }

    [Fact]
    public void GetTotalMacros_ShouldSumMealsMacros()
    {   
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        // Meal 1: one dish => 10/20/5
        var meal1 = new Meal(MealType.Meal, "Meal 1", DateTimeOffset.UtcNow);
        meal1.AddDish(CreateSimpleDish("Dish 1", protein: 10m, carbs: 20m, fat: 5m, fiber: 0m, salt: 0m, grams: 100));

        // Meal 2: one dish => 5/0/10
        var meal2 = new Meal(MealType.Meal, "Meal 2", DateTimeOffset.UtcNow);
        meal2.AddDish(CreateSimpleDish("Dish 2", protein: 5m, carbs: 0m, fat: 10m, fiber: 0m, salt: 0m, grams: 100));

        log.AddMeal(meal1);
        log.AddMeal(meal2);

        var total = log.GetTotalMacros();

        Assert.Equal(new Macronutrients(15m, 20m, 15m, 0m, 0m), total);
    }

    [Fact]
    public void GetTotalCalories_ShouldUseMacrosCaloriesRounding()
    {   
        var userId = Guid.NewGuid();
        var log = new DailyLog(userId, new DateOnly(2026, 2, 21));

        // Create a meal that totals 82.5 kcal so it rounds to 83
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        var dish = new Dish("Test dish");
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50))); // 82.5 -> 83

        var meal = new Meal(MealType.Meal, "Meal", DateTimeOffset.UtcNow);
        meal.AddDish(dish);

        log.AddMeal(meal);

        Assert.Equal(83, log.GetTotalCalories());
    }

    private static Dish CreateSimpleDish(string name, decimal protein, decimal carbs, decimal fat, decimal fiber, decimal salt, int grams)
    {
        var ingredient = new Ingredient("Ingredient", new Macronutrients(protein, carbs, fat, fiber, salt));
        var dish = new Dish(name);

        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(grams)));

        return dish;
    }
}