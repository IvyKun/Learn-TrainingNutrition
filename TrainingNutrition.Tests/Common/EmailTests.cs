using TrainingNutrition.Domain.Common;

public sealed class EmailTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid(string value)
    {
        var act = () => new Email(value);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_ShouldCreateEmail_WhenValueIsValid()
    {
        var email = new Email("ivan@test.com");

        Assert.Equal("ivan@test.com", email.Value);
    }

    [Fact]
    public void Emails_ShouldBeEqual_WhenValuesAreEqual()
    {
        var email1 = new Email("ivan@test.com");
        var email2 = new Email("ivan@test.com");

        Assert.Equal(email1, email2);
    }

}