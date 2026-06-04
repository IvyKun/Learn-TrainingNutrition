namespace TrainingNutrition.Domain.Common;

public sealed record Macronutrients
{
    public decimal Protein { get; }
    public decimal Carbs { get; }
    public decimal Fat { get; }
    public decimal Fiber { get; }
    public decimal Salt { get; }

    public int Calories
    {
        get
        {
            decimal raw = (Protein * 4m) + (Carbs * 4m) + (Fat * 9m) + (Fiber * 2m);
            return (int)Math.Round(raw, MidpointRounding.AwayFromZero);
        }
    }

    public Macronutrients(decimal protein, decimal carbs, decimal fat, decimal fiber, decimal salt)
    {
        if (protein < 0m)
            throw new ArgumentOutOfRangeException(nameof(protein), "Protein cannot be negative.");

        if (carbs < 0m)
            throw new ArgumentOutOfRangeException(nameof(carbs), "Carbs cannot be negative.");

        if (fat < 0m)
            throw new ArgumentOutOfRangeException(nameof(fat), "Fat cannot be negative.");

        if (fiber < 0m)
            throw new ArgumentOutOfRangeException(nameof(fiber), "Fiber cannot be negative.");

        if (salt < 0m)
            throw new ArgumentOutOfRangeException(nameof(salt), "Salt cannot be negative.");

        Protein = protein;
        Carbs = carbs;
        Fat = fat;
        Fiber = fiber;
        Salt = salt;
    }

    public Macronutrients Add(Macronutrients other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        return new Macronutrients(
            Protein + other.Protein,
            Carbs + other.Carbs,
            Fat + other.Fat,
            Fiber + other.Fiber,
            Salt + other.Salt
        );
    }
}
