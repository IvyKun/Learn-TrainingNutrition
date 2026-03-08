using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Tests.Common;

public sealed class MacronutrientsTests
{
    [Theory]
    [InlineData(-0.1, 0, 0)]
    [InlineData(0, -0.1, 0)]
    [InlineData(0, 0, -0.1)]
    public void Constructor_ShouldThrow_WhenAnyMacroIsNegative(decimal protein, decimal carbs, decimal fat)
    {
        var act = () => new Macronutrients(protein, carbs, fat);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_ShouldCreate_WhenValuesAreValid()
    {
        var macros = new Macronutrients(10m, 20m, 5m);

        Assert.Equal(10m, macros.Protein);
        Assert.Equal(20m, macros.Carbs);
        Assert.Equal(5m, macros.Fat);
    }

    [Theory]
    [InlineData(0, 0, 2.5, 23)]     // 22.5 -> 23 (AwayFromZero)
    [InlineData(0, 0, 2.4, 22)]     // 21.6 -> 22
    [InlineData(0, 0, 2.6, 23)]     // 23.4 -> 23
    [InlineData(1, 0, 0, 4)]
    [InlineData(0, 1, 0, 4)]
    [InlineData(0, 0, 1, 9)]
    public void Calories_ShouldRound_AwayFromZero(decimal protein, decimal carbs, decimal fat, int expectedCalories)
    {
        var macros = new Macronutrients(protein, carbs, fat);

        Assert.Equal(expectedCalories, macros.Calories);
    }

    [Fact]
    public void Add_ShouldThrow_WhenOtherIsNull()
    {
        var macros = new Macronutrients(10m, 20m, 5m);

        var act = () => macros.Add(null!);

        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Add_ShouldReturnNewMacronutrients_WithSummedValues()
    {
        var a = new Macronutrients(10m, 20m, 5m);
        var b = new Macronutrients(5m, 0m, 10m);

        var result = a.Add(b);

        Assert.Equal(new Macronutrients(15m, 20m, 15m), result);
        Assert.Equal(275, result.Calories);
    }

    [Fact]
    public void Records_ShouldBeEqual_WhenValuesAreEqual()
    {
        var a = new Macronutrients(10m, 20m, 5m);
        var b = new Macronutrients(10m, 20m, 5m);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Records_ShouldNotBeEqual_WhenValuesDiffer()
    {
        var a = new Macronutrients(10m, 20m, 5m);
        var b = new Macronutrients(10m, 20.1m, 5m);

        Assert.NotEqual(a, b);
    }
}
