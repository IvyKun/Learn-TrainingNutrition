using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Domain.Ingredients;

public sealed class IngredientEntry
{   
    public Guid Id { get; private init; } = Guid.NewGuid();
    
    public Ingredient Ingredient { get; }
    public Grams Grams { get; }

    public Macronutrients Macros => ScaleMacros(Ingredient.MacrosPer100g, Grams);
    public int Calories => Macros.Calories;

    private IngredientEntry() { } // For EF Core only

    public IngredientEntry(Ingredient ingredient, Grams grams)
    {
        Ingredient = ingredient ?? throw new ArgumentNullException(nameof(ingredient));
        Grams = grams ?? throw new ArgumentNullException(nameof(grams));
    }

    private static Macronutrients ScaleMacros(Macronutrients per100g, Grams grams)
    {
        decimal factor = grams.Value / 100m;

        return new Macronutrients(
            per100g.Protein * factor,
            per100g.Carbs * factor,
            per100g.Fat * factor,
            per100g.Fiber * factor,
            per100g.Salt * factor

        );
    }
}
