
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
          
            var command = new RegisterCommand(request.Email, request.Password);
            var id = await mediator.Send(command, cancellationToken);
            return Results.Created($"/auth/register/{id}", id);

        })
        .WithName("RegisterUser")
        .WithTags("Auth")
        .WithSummary("Create a new user")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status400BadRequest);


        // POST /auth/login
        app.MapPost("/auth/login", async (
            LoginRequest request, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new LoginCommand(request.Email, request.Password);
                var token = await mediator.Send(command, cancellationToken);
                return Results.Ok(token);
            }
            catch (InvalidOperationException)
            {
                return Results.Problem(detail: "Invalid credentials", statusCode: StatusCodes.Status401Unauthorized);
            }

        })
        .WithName("LoginUser")
        .WithTags("Auth")
        .WithSummary("User Login")
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized);
        
    }

    
}