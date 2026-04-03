using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Dishes;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Tests.Dishes;

public sealed class DishTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var act = () => new Dish(name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_ShouldTrimName()
    {
        var dish = new Dish("  Porridge  ");

        Assert.Equal("Porridge", dish.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var dish = new Dish("Porridge");

        var act = () => dish.Rename(name);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AddIngredient_ShouldThrow_WhenEntryIsNull()
    {
        var dish = new Dish("Porridge");

        var act = () => dish.AddIngredient(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetTotalMacros_ShouldSumEntryMacros()
    {
        // Ingredient: per 100g => 10P / 20C / 5F
        // Entry 1: 50g => 5P / 10C / 2.5F
        // Entry 2: 50g => 5P / 10C / 2.5F
        // Total: 10P / 20C / 5F
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));

        var dish = new Dish("Test dish");
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50)));
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50)));

        var total = dish.GetTotalMacros();

        Assert.Equal(new Macronutrients(10m, 20m, 5m, 0m, 0m), total);
    }

    [Fact]
    public void GetTotalCalories_ShouldUseMacrosCaloriesRounding()
    {
        // Same as above => total macros 10/20/5 => calories 165
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));

        var dish = new Dish("Test dish");
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50)));
        dish.AddIngredient(new IngredientEntry(ingredient, new Grams(50)));

        var calories = dish.GetTotalCalories();

        Assert.Equal(165, calories);
    }

    [Fact]
    public void RemoveIngredient_ShouldRemoveAllMatchingEntries_AndReturnCountRemoved()
    {
        var oats = new Ingredient("Oats", new Macronutrients(13m, 60m, 7m, 0m, 0m));
        var milk = new Ingredient("Milk", new Macronutrients(3.4m, 5m, 1.5m, 0m, 0m));

        var dish = new Dish("Porridge");
        dish.AddIngredient(new IngredientEntry(oats, new Grams(80)));
        dish.AddIngredient(new IngredientEntry(milk, new Grams(200)));
        dish.AddIngredient(new IngredientEntry(oats, new Grams(10)));

        var removed = dish.RemoveIngredient(oats);

        Assert.Equal(2, removed);
        Assert.Single(dish.Entries);
        Assert.Equal(milk, dish.Entries[0].Ingredient);
    }
}
