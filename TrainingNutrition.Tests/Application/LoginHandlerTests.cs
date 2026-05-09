
using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Auth;

namespace TrainingNutrition.Tests.Application;

public sealed class LoginHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCommandIsValid()
    {
        // Arrange

        var testToken = "test-jwt-token";
        string email = "test@test.com";
        string password = "Passsword1234.";

        var mockRepo = new Mock<IIdentityService>();
        mockRepo.Setup(r => r.LoginAsync(email, password))
                .ReturnsAsync(testToken);

        var handler = new LoginHandler(mockRepo.Object);
        var command = new LoginCommand(email, password);

        // Act
        var userToken = await handler.Handle(command, CancellationToken.None);

        // Assert
         Assert.Equal(testToken, userToken);
      
    }

    [Fact]
    public async Task Handle_ThrowsInvalidOperationException_WhenCredentialsFails()
    {

        // Arrange

        string email = "test@test.com";
        string password = "Passsword1234.";

        var mockRepo = new Mock<IIdentityService>();
        mockRepo.Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

        var handler = new LoginHandler(mockRepo.Object);
        var command = new LoginCommand(email, password);

        // Act & Assert

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
