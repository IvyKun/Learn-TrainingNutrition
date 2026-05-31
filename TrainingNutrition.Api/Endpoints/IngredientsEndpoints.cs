
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
            var command = new CreateIngredientCommand(request.Name, request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt, request.Brand);
            var id = await mediator.Send(command, cancellationToken);

            return Results.Created($"/ingredients/{id}", id);

        })
        .WithName("CreateIngredient")
        .WithTags("Ingredients")
        .WithSummary("Create a new ingredient")
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .RequireAuthorization();

        // PUT /ingredients/{id}
        app.MapPut("/ingredients/{id}", async (
            Guid id,
            UpdateIngredientRequest request, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateIngredientCommand(id, request.Name, request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt, request.Brand);
            await mediator.Send(command, cancellationToken);

            return Results.NoContent();

        })
        .WithName("UpdateIngredient")
        .WithTags("Ingredients")
        .WithSummary("Update an ingredient")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .RequireAuthorization();

         // DELETE /ingredients/{id}
        app.MapDelete("/ingredients/{id}", async (
            Guid id,
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteIngredientCommand(id);
            await mediator.Send(command, cancellationToken);

            return Results.NoContent();

        })
        .WithName("DeleteIngredient")
        .WithTags("Ingredients")
        .WithSummary("Delete an ingredient")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .RequireAuthorization();

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
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // GET /ingredients?search={searchTerm}
        app.MapGet("/ingredients", async (
            string? searchTerm, 
            IMediator mediator, 
            CancellationToken cancellationToken) =>
        {
            var query = new GetIngredientsQuery(searchTerm);
            var ingredientResponse = await mediator.Send(query, cancellationToken);

            return Results.Ok(ingredientResponse);

        })
        .WithName("GetIngredients")
        .WithTags("Ingredients")
        .WithSummary("Get all ingredients that match a search term or the whole list")
        .Produces<IReadOnlyList<IngredientResponse>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}