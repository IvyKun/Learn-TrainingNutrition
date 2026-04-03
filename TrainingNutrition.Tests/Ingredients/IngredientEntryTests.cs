using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Tests.Ingredients;

public sealed class IngredientEntryTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenIngredientIsNull()
    {
        var act = () => new IngredientEntry(null!, new Grams(50));

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenGramsIsNull()
    {
        var ingredient = new Ingredient("Oats", new Macronutrients(13m, 60m, 7m, 0m, 0m));

        var act = () => new IngredientEntry(ingredient, null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Macros_ShouldBeScaled_FromPer100g()
    {
        // Per 100g: 10P / 20C / 5F
        // For 50g:  5P / 10C / 2.5F
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        var entry = new IngredientEntry(ingredient, new Grams(50));

        Assert.Equal(new Macronutrients(5m, 10m, 2.5m, 0m, 0m), entry.Macros);
    }

    [Fact]
    public void Calories_ShouldRound_AwayFromZero()
    {
        // For 50g: 5P=20, 10C=40, 2.5F=22.5 => 82.5 -> 83
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));
        var entry = new IngredientEntry(ingredient, new Grams(50));

        Assert.Equal(83, entry.Calories);
    }

    [Fact]
    public void Entries_ShouldBeEqual_WhenValuesAreEqual()
    {
        var ingredient = new Ingredient("Rice", new Macronutrients(2m, 28m, 0m, 0m, 0m));

        var a = new IngredientEntry(ingredient, new Grams(80));
        var b = new IngredientEntry(ingredient, new Grams(80));

        Assert.Equal(a, b);
    }
}
