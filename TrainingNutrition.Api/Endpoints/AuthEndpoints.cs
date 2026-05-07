
using MediatR;
using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Application.Auth;

namespace TrainingNutrition.Api.Endpoints;

public static class AuthEndpoints
{
      public static void MapAuthEndpoints(this WebApplication app)
    {
        // POST /auth/register
        app.MapPost("/auth/register", async (
            RegisterRequest request, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new RegisterCommand(request.Email, request.Password);
                var id = await mediator.Send(command, cancellationToken);
                return Results.Created($"/auth/register/{id}", id);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }

        })
        .WithName("RegisterUser")
        .WithTags("Auth")
        .WithSummary("Create a new user")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status400BadRequest);
        
    }
}