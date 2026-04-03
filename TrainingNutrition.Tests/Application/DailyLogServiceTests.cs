using TrainingNutrition.Application.Infrastructure;
using TrainingNutrition.Application.Services;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Tests.Application;

public sealed class DailyLogServiceTests
{
    [Fact]
    public async Task GetOrCreateAsync_ShouldCreateLog_WhenNotExists()
    {
        var repository = new InMemoryDailyLogRepository();
        var service = new DailyLogService(repository);

        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 21);

        var log = await service.GetOrCreateAsync(userId, date);

        Assert.NotNull(log);
        Assert.Equal(date, log.Date);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnSameInstance_WhenAlreadyExists()
    {
        var repository = new InMemoryDailyLogRepository();
        var service = new DailyLogService(repository);

         var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 21);

        var first = await service.GetOrCreateAsync(userId, date);
        var second = await service.GetOrCreateAsync(userId, date);

        Assert.Same(first, second);
    }
}