
using MediatR;
using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Application.Ingredients;

namespace TrainingNutrition.Api.Endpoints;

public static class IngredientsEndpoints
{
    public static void MapIngredientsEndpoints(this WebApplication app)
    {
        // POST /ingredients
        app.MapPost("/ingredients", async (
            CreateIngredientRequest request, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            var command = new CreateIngredientCommand(request.Name, request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt);
            var id = await mediator.Send(command, cancellationToken);

            return Results.Created($"/ingredients/{id}", id);

        })
        .WithName("CreateIngredient")
        .WithTags("Ingredients")
        .WithSummary("Create a new ingredient")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        // GET /ingredients/{id}
        app.MapGet("/ingredients/{id}", async (
            Guid id, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            var query = new GetIngredientByIdQuery(id);
            var ingredientResponse = await mediator.Send(query, cancellationToken);

            if(ingredientResponse == null)
            {
                return Results.NotFound();    
            }

            return Results.Ok(ingredientResponse);

        })
        .WithName("GetIngredient")
        .WithTags("Ingredients")
        .WithSummary("Get an ingredient")
        .Produces<IngredientResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}