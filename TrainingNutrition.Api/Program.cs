using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingNutrition.Api.DTOs;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Behaviors;
using TrainingNutrition.Application.Infrastructure;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Application.Services;
using TrainingNutrition.Infrastructure;
using TrainingNutrition.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (keep it)
builder.Services.AddOpenApi();

// DI registrations
builder.Services.AddSingleton<IDailyLogRepository, InMemoryDailyLogRepository>();
builder.Services.AddScoped<DailyLogService>();
builder.Services.AddScoped<IIngredientRepository, EfIngredientRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateIngredientHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateIngredientCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

   // TODO: replace with authenticated user ID from JWT token (Phase 4 - Auth)
    var tempUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    var log = await service.GetOrCreateAsync(tempUserId, parsedDate, cancellationToken);

    return Results.Ok(new DailyLogResponse(log.Date, log.GetTotalCalories()));
})
.WithName("GetDailyLog");

// POST /ingredients
app.MapPost("/ingredients", async (CreateIngredientRequest request, 
IMediator mediator, 
CancellationToken cancellationToken) =>
{
    var command = new CreateIngredientCommand(request.Name, request.Protein, request.Carbs, request.Fat, request.Fiber, request.Salt);
    var id = await mediator.Send(command, cancellationToken);

    return Results.Created($"/ingredients/{id}", id);

});

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

});

app.Run();