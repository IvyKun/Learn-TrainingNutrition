
using MediatR;
using TrainingNutrition.Application.Abstractions;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Ingredients;

namespace TrainingNutrition.Application.DailyLogs;

public class AddIngredientEntryHandler : IRequestHandler<AddIngredientEntryCommand>
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public AddIngredientEntryHandler(IDailyLogRepository dailyLogRepository, IIngredientRepository ingredientRepository)
    {
        _dailyLogRepository = dailyLogRepository;
        _ingredientRepository = ingredientRepository;
    }

    public async Task Handle(AddIngredientEntryCommand request, CancellationToken cancellationToken)
    {
        var dailyLog = await _dailyLogRepository.GetByDateAsync(request.UserId, request.Date, cancellationToken);

        if(dailyLog == null)
        {
            throw new NotFoundException("Daily Log not found");
        }

        var meal = dailyLog.Meals.FirstOrDefault(m => m.Id == request.MealId);

        if(meal == null)
        {
            throw new NotFoundException("Meal not found");
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken);

        if(ingredient == null)
        {
            throw new NotFoundException("Ingredient not found");
        }

        var ingredientEntry = new IngredientEntry(ingredient, new Grams(request.Grams));
        
        meal.AddIngredientEntry(ingredientEntry);

        await _dailyLogRepository.UpdateAsync(dailyLog, cancellationToken);

    }
}