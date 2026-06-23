

using System.Security.Claims;
using MediatR;
using TrainingNutrition.Api.DTOs;
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
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.InternalServerError();
            }

            var guidUserId = Guid.Parse(userId);

            var command = new GetOrCreateDailyLogCommand(guidUserId, parsedDate);
            var dailyLogResponse = await mediator.Send(command, cancellationToken);

            return Results.Ok(dailyLogResponse);
        })
        .WithName("GetDailyLog")
        .WithTags("DailyLog")
        .WithSummary("Get a DailyLog")
        .Produces<DailyLogResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

         // POST /dailylogs/2026-02-21/meals/000-000-000/ingredients
        app.MapPost("/dailylogs/{date}/meals/{mealId}/ingredients", async (
            string date,
            Guid mealId,
            AddIngredientEntryRequest request, 
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.InternalServerError();
            }

            var guidUserId = Guid.Parse(userId);

            var command = new AddIngredientEntryCommand(guidUserId, parsedDate, mealId, request.IngredientId, request.Grams);
            await mediator.Send(command, cancellationToken);

            return Results.NoContent();
        })
        .WithName("AddIngredientEntry")
        .WithTags("DailyLog")
        .WithSummary("Add an ingredient entry to a meal")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();
        
        
    }
}