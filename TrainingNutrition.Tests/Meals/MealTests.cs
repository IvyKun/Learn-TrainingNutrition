using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Dishes;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;

namespace TrainingNutrition.Tests.Meals;

public sealed class MealTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var act = () => new Meal(MealType.Meal, name, DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_ShouldTrimName()
    {
        var meal = new Meal(MealType.Meal, "  Breakfast  ", DateTimeOffset.UtcNow);

        Assert.Equal("Breakfast", meal.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var meal = new Meal(MealType.Meal, "Breakfast", DateTimeOffset.UtcNow);

        var act = () => meal.Rename(name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AddDish_ShouldThrow_WhenDishIsNull()
    {
        var meal = new Meal(MealType.Meal, "Breakfast", DateTimeOffset.UtcNow);

        var act = () => meal.AddDish(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Meal_ShouldAllowDuplicateDishInstances()
    {
        var meal = new Meal(MealType.Meal, "Breakfast", DateTimeOffset.UtcNow);

        var dish = CreateSimpleDish("Porridge", protein: 10m, carbs: 20m, fat: 5m, grams: 100);

        meal.AddDish(dish);
        meal.AddDish(dish);

        Assert.Equal(2, meal.Dishes.Count);
        Assert.Same(dish, meal.Dishes[0]);
        Assert.Same(dish, meal.Dishes[1]);
    }

    [Fact]
    public void RemoveDish_ShouldRemoveOnlyMatchingReference_AndReturnCountRemoved()
    {
        var meal = new Meal(MealType.Meal, "Breakfast", DateTimeOffset.UtcNow);

        var dishA = CreateSimpleDish("Dish A", protein: 10m, carbs: 0m, fat: 0m, grams: 100);
        var dishB = CreateSimpleDish("Dish B", protein: 0m, carbs: 10m, fat: 0m, grams: 100);

        meal.AddDish(dishA);
        meal.AddDish(dishA); // same reference twice
        meal.AddDish(dishB);

        var removed = meal.RemoveDish(dishA);

        Assert.Equal(2, removed);
        Assert.Single(meal.Dishes);
        Assert.Same(dishB, meal.Dishes[0]);
    }

    [Fact]
    public void GetTotalMacros_ShouldSumDishMacros()
    {
        var meal = new Meal(MealType.Meal, "Lunch", DateTimeOffset.UtcNow);

        var dish1 = CreateSimpleDish("Dish 1", protein: 10m, carbs: 20m, fat: 5m, grams: 100); // 165 kcal
        var dish2 = CreateSimpleDish("Dish 2", protein: 5m, carbs: 0m, fat: 10m, grams: 100);  // 110 kcal

        meal.AddDish(dish1);
        meal.AddDish(dish2);

        var total = meal.GetTotalMacros();

        Assert.Equal(new Macronutrients(15m, 20m, 15m, 0m, 0m), total);
    }

    [Fact]
    public void GetTotalCalories_ShouldUseMacrosCaloriesRounding()
    {
        var meal = new Meal(MealType.Meal, "Lunch", DateTimeOffset.UtcNow);

        // Make a dish that results in 82.5 kcal so we can confirm AwayFromZero -> 83
        // Per 100g: 10P / 20C / 5F, with 50g => 5P / 10C / 2.5F => 82.5 -> 83
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        var dish = new Dish("Test dish");
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50)));

        meal.AddDish(dish);

        Assert.Equal(83, meal.GetTotalCalories());
    }

    private static Dish CreateSimpleDish(string name, decimal protein, decimal carbs, decimal fat, int grams)
    {
        var ingredient = new Ingredient("Ingredient", new Macronutrients(protein, carbs, fat, 0m, 0m));
        var dish = new Dish(name);

        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(grams)));

        return dish;
    }
}
