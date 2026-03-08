namespace TrainingNutrition.Domain.Common;

public sealed record Grams
{
    public int Value { get; }

    public Grams(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Grams cannot be negative.");

        Value = value;
    }

    public override string ToString() => $"{Value} g";
}
