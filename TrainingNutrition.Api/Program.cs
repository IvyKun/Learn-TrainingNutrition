using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Infrastructure;
using TrainingNutrition.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (keep it)
builder.Services.AddOpenApi();

// DI registrations
builder.Services.AddSingleton<IDailyLogRepository, InMemoryDailyLogRepository>();
builder.Services.AddScoped<DailyLogService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

// GET /dailylogs/2026-02-21
app.MapGet("/dailylogs/{date}", async (
    string date,
    DailyLogService service,
    CancellationToken cancellationToken) =>
{
    if (!DateOnly.TryParse(date, out var parsedDate))
        return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

    var log = await service.GetOrCreateAsync(parsedDate, cancellationToken);

    return Results.Ok(new DailyLogResponse(log.Date, log.GetTotalCalories()));
})
.WithName("GetDailyLog");

app.Run();