using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Tests.Ingredients;

public sealed class IngredientTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string name)
    {
        var macros = new Macronutrients(10m, 20m, 5m, 0m, 0m);

        var act = () => new Ingredient(name, macros);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMacrosAreNull()
    {
        var act = () => new Ingredient("Oats", null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Constructor_ShouldTrimName()
    {
        var ingredient = new Ingredient("  Rice  ", new Macronutrients(2m, 28m, 0m, 0m, 0m));

        Assert.Equal("Rice", ingredient.Name);
    }

    [Fact]
    public void CaloriesPer100g_ShouldMatchMacrosCalories()
    {
        var ingredient = new Ingredient("Test", new Macronutrients(10m, 20m, 5m, 0m, 0m));

        Assert.Equal(165, ingredient.CaloriesPer100g);
    }
}
