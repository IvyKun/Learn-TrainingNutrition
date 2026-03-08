using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Users;

namespace TrainingNutrition.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenIdIsEmpty()
    {
        // Arrange
        var email = new Email("ivan@test.com");

        // Act
        var act = () => new User(Guid.Empty, "Ivan", email);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("")]

// If your test runner doesn't allow empty InlineData, use " " only.
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenNameIsInvalid(string name)
    {
        // Arrange
        var email = new Email("ivan@test.com");

        // Act
        var act = () => new User(Guid.NewGuid(), name, email);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailIsNull()
    {
        // Act
        var act = () => new User(Guid.NewGuid(), "Ivan", null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ChangeName_ShouldUpdateName_WhenValid()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "Ivan", new Email("ivan@test.com"));

        // Act
        user.ChangeName("Ivan Calle");

        // Assert
        Assert.Equal("Ivan Calle", user.Name);
    }

    [Fact]
    public void ChangeEmail_ShouldUpdateEmail_WhenValid()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "Ivan", new Email("ivan@test.com"));
        var newEmail = new Email("ivan.calle@test.com");

        // Act
        user.ChangeEmail(newEmail);

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void ChangeEmail_ShouldThrow_WhenEmailIsNull()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "Ivan", new Email("ivan@test.com"));

        // Act
        var act = () => user.ChangeEmail(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
}
