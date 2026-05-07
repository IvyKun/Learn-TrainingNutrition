
using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Auth;

namespace TrainingNutrition.Tests.Application;

public sealed class RegisterHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserId_WhenCommandIsValid()
    {
        // Arrange

        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        string email = "test@test.com";
        string password = "Passsword1234.";

        var mockRepo = new Mock<IIdentityService>();
        mockRepo.Setup(r => r.RegisterAsync(email, password))
                .ReturnsAsync(testUserId);

        var handler = new RegisterHandler(mockRepo.Object);
        var command = new RegisterCommand(email, password);

        // Act
        var userId = await handler.Handle(command, CancellationToken.None);

        // Assert

         Assert.NotEqual(userId, Guid.Empty);
         Assert.Equal(userId, testUserId);
      
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenIdentityFails_()
    {

        // Arrange

        string email = "test@test.com";
        string password = "Passsword1234.";

        var mockRepo = new Mock<IIdentityService>();
        mockRepo.Setup(r => r.RegisterAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Email is already taken."));

        var handler = new RegisterHandler(mockRepo.Object);
        var command = new RegisterCommand(email, password);

        // Act & Assert

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
