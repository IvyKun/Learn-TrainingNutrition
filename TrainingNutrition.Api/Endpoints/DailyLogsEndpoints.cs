

using MediatR;
using TrainingNutrition.Application.DailyLogs;

namespace TrainingNutrition.Api.Endpoints;

public static class DailyLogsEndpoints
{
    public static void MapDailyLogsEndpoints(this WebApplication app)
    {
        // GET /dailylogs/2026-02-21
        app.MapGet("/dailylogs/{date}", async (
            string date,
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

        // TODO: replace with authenticated user ID from JWT token (Phase 4 - Auth)
            var tempUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            var command = new GetOrCreateDailyLogCommand(tempUserId, parsedDate);
            var dailyLogResponse = await mediator.Send(command, cancellationToken);

            return Results.Ok(dailyLogResponse);
        })
        .WithName("GetDailyLog")
        .WithTags("DailyLog")
        .WithSummary("Get a DailyLog")
        .Produces<DailyLogResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
        
    }
}