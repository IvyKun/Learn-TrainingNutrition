using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TrainingNutrition.Api;
using TrainingNutrition.Api.Endpoints;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Application.Behaviors;
using TrainingNutrition.Application.Ingredients;
using TrainingNutrition.Infrastructure;
using TrainingNutrition.Infrastructure.Identity;
using TrainingNutrition.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (keep it)
builder.Services.AddOpenApi();

// DI registrations
builder.Services.AddScoped<IDailyLogRepository, EfDailyLogRepository>();
builder.Services.AddScoped<IIngredientRepository, EfIngredientRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateIngredientHandler).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<CreateIngredientCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddIdentityCore<AppUser>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapIngredientsEndpoints();
app.MapDailyLogsEndpoints();
app.MapAuthEndpoints();

app.Run();



public partial class Program { }
