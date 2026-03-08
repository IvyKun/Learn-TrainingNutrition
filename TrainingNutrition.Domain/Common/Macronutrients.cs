namespace TrainingNutrition.Domain.Common;

public sealed record Macronutrients
{
    public decimal Protein { get; }
    public decimal Carbs { get; }
    public decimal Fat { get; }

    public int Calories
    {
        get
        {
            decimal raw = (Protein * 4m) + (Carbs * 4m) + (Fat * 9m);
            return (int)Math.Round(raw, MidpointRounding.AwayFromZero);
        }
    }

    public Macronutrients(decimal protein, decimal carbs, decimal fat)
    {
        if (protein < 0m)
            throw new ArgumentOutOfRangeException(nameof(protein), "Protein cannot be negative.");

        if (carbs < 0m)
            throw new ArgumentOutOfRangeException(nameof(carbs), "Carbs cannot be negative.");

        if (fat < 0m)
            throw new ArgumentOutOfRangeException(nameof(fat), "Fat cannot be negative.");

        Protein = protein;
        Carbs = carbs;
        Fat = fat;
    }

    public Macronutrients Add(Macronutrients other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        return new Macronutrients(
            Protein + other.Protein,
            Carbs + other.Carbs,
            Fat + other.Fat
        );
    }
}
