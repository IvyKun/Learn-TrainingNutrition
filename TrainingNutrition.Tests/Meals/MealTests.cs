using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;
using TrainingNutrition.Domain.Meals;

namespace TrainingNutrition.Tests.Meals;

public sealed class MealTests
{
    
    [Fact]
    public void AddIngredientEntry_ShouldThrow_WhenEntryIsNull()
    {
        var meal = new Meal(MealType.Breakfast);

        var act = () => meal.AddIngredientEntry(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Meal_ShouldAllowDuplicateIngredientEntryInstances()
    {
        var meal = new Meal(MealType.Breakfast);

        var ingredientEntry = CreateSimpleIngredientEntry("IngredientEntry", protein: 10m, carbs: 20m, fat: 5m, grams: 100);

        meal.AddIngredientEntry(ingredientEntry);
        meal.AddIngredientEntry(ingredientEntry);
 
        Assert.Equal(2, meal.IngredientEntries.Count);
        Assert.Same(ingredientEntry, meal.IngredientEntries[0]);
        Assert.Same(ingredientEntry, meal.IngredientEntries[1]);
    }

    [Fact]
    public void RemoveIngredientEntry_ShouldRemoveOnlyMatchingReference_AndReturnTrue()
    {
        var meal = new Meal(MealType.Breakfast);

        var ingredientEntryA = CreateSimpleIngredientEntry("IngredientEntry A", protein: 10m, carbs: 20m, fat: 5m, grams: 100);
        var ingredientEntryA2 = CreateSimpleIngredientEntry("IngredientEntry A2", protein: 10m, carbs: 20m, fat: 5m, grams: 100);
        var ingredientEntryB = CreateSimpleIngredientEntry("IngredientEntry B", protein: 0m, carbs: 10m, fat: 0m, grams: 100);

        meal.AddIngredientEntry(ingredientEntryA);
        meal.AddIngredientEntry(ingredientEntryA2);
        meal.AddIngredientEntry(ingredientEntryB);

        var removed = meal.RemoveIngredientEntry(ingredientEntryA2.Id);

        Assert.True(removed);
        Assert.Equal(2, meal.IngredientEntries.Count);
        Assert.DoesNotContain(ingredientEntryA2, meal.IngredientEntries);
        Assert.Contains(ingredientEntryA, meal.IngredientEntries);
        Assert.Contains(ingredientEntryB, meal.IngredientEntries);
    }

    [Fact]
    public void RemoveIngredientEntry_ShouldReturnFalseIfNotFound()
    {
        var meal = new Meal(MealType.Breakfast);

        var removed = meal.RemoveIngredientEntry(Guid.NewGuid());

        Assert.False(removed);
    }

    [Fact]
    public void GetTotalMacros_ShouldSumIngredientEntryMacros()
    {
        var meal = new Meal(MealType.Breakfast);

        var ingredientEntryA = CreateSimpleIngredientEntry("IngredientEntry A", protein: 10m, carbs: 20m, fat: 5m, grams: 100); // 165 kcal
        var ingredientEntryB = CreateSimpleIngredientEntry("IngredientEntry B", protein: 0m, carbs: 10m, fat: 0m, grams: 100); // 40 kcal

        meal.AddIngredientEntry(ingredientEntryA);
        meal.AddIngredientEntry(ingredientEntryB);

        var total = meal.GetTotalMacros();

        Assert.Equal(new Macronutrients(10m, 30m, 5m, 0m, 0m), total);
    }

    [Fact]
    public void GetTotalCalories_ShouldUseMacrosCaloriesRounding()
    {
        var meal = new Meal(MealType.Breakfast);

        // Make an ingredient entry that results in 82.5 kcal so we can confirm AwayFromZero -> 83
        // Per 100g: 10P / 20C / 5F, with 50g => 5P / 10C / 2.5F => 82.5 -> 83
        var ingredientEntry = CreateSimpleIngredientEntry("IngredientEntry", protein: 10m, carbs: 20m, fat: 5m, grams: 50);

        meal.AddIngredientEntry(ingredientEntry);

        Assert.Equal(83, meal.GetTotalCalories());
    }

    private static IngredientEntry CreateSimpleIngredientEntry(string name, decimal protein, decimal carbs, decimal fat, int grams)
    {
        var ingredient = new Ingredient(name, new Macronutrients(protein, carbs, fat, 0m, 0m));
        var ingredientEntry = new IngredientEntry(ingredient, new Grams(grams));

        return ingredientEntry;
    }

}
