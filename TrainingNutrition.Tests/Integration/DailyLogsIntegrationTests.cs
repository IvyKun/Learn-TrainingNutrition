
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TrainingNutrition.Application.DailyLogs;
using TrainingNutrition.Infrastructure;

namespace TrainingNutrition.Tests.Integration;

public class DailyLogsIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
     private readonly CustomWebApplicationFactory _factory;

    public DailyLogsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    
    public async Task InitializeAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DailyLogs.RemoveRange(db.DailyLogs);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetDailyLog_ValidDate_DontExists_Returns200WithData()
    {
        // Arrange
        var dateString = "2026-05-02";

        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/dailylogs/{dateString}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        DailyLogResponse? dailyLogResponse = await response.Content.ReadFromJsonAsync<DailyLogResponse?>();
        Assert.NotNull(dailyLogResponse);
    }

    [Fact]
    public async Task GetDailyLog_ValidDate_Exists_Returns200WithData()
    {
        // Arrange
        var dateString = "2026-05-02";

        // Act
        HttpResponseMessage responseCreate = await _httpClient.GetAsync($"/dailylogs/{dateString}");
        HttpResponseMessage responseGet = await _httpClient.GetAsync($"/dailylogs/{dateString}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseCreate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);

        DailyLogResponse? dailyLogResponseCreate = await responseCreate.Content.ReadFromJsonAsync<DailyLogResponse?>();
        DailyLogResponse? dailyLogResponseGet = await responseGet.Content.ReadFromJsonAsync<DailyLogResponse?>();
        Assert.NotNull(dailyLogResponseCreate);
        Assert.NotNull(dailyLogResponseGet);
        Assert.Equal(dailyLogResponseCreate.Date, dailyLogResponseGet.Date);
    }

    [Fact]
    public async Task GetDailyLog_InvalidDate_Returns400()
    {
        // Arrange
        var dateString = "2026-05-";

        // Act
        HttpResponseMessage response = await _httpClient.GetAsync($"/dailylogs/{dateString}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


}