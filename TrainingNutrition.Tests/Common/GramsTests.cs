using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Tests.Common;

public sealed class GramsTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenValueIsNegative()
    {
        var act = () => new Grams(-1);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void Constructor_ShouldCreate_WhenValueIsZeroOrPositive()
    {
        var zero = new Grams(0);
        var positive = new Grams(80);

        Assert.Equal(0, zero.Value);
        Assert.Equal(80, positive.Value);
    }
}
