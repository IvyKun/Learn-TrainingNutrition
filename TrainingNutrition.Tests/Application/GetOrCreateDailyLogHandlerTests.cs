using Moq;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.DailyLogs;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Tests.Application;

public sealed class GetOrCreateDailyLogHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnDailyLog_WhenExists()
    {

       // Arrange
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(dailyLog.Date, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dailyLog);

        var handler = new GetOrCreateDailyLogHandler(mockRepo.Object);
        var command = new GetOrCreateDailyLogCommand(dailyLog.UserId, dailyLog.Date);

        // Act
        var dailyLogResponse = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(dailyLogResponse);
        Assert.Equal(dailyLog.Date, dailyLogResponse.Date);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Never);
      
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnDailyLog_WhenNotExists()
    {

        // Arrange
        var testUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var testDate = DateOnly.MinValue;

        var dailyLog = new DailyLog(testUserId, testDate);

        var mockRepo = new Mock<IDailyLogRepository>();
        mockRepo.Setup(r => r.GetByDateAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DailyLog?)null);

        var handler = new GetOrCreateDailyLogHandler(mockRepo.Object);
        var command = new GetOrCreateDailyLogCommand(dailyLog.UserId, dailyLog.Date);

        // Act
        var dailyLogResponse = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(dailyLogResponse);
        Assert.Equal(dailyLog.Date, dailyLogResponse.Date);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<DailyLog>(), It.IsAny<CancellationToken>()), Times.Once);
      
    }
}