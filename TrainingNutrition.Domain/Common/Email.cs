using System.Net.NetworkInformation;

namespace TrainingNutrition.Domain.Common;

public sealed record Email
{
    public string Value { get;}

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(value));
        }

        value = value.Trim();

        if(!value.Contains("@") || value.Length > 254)
        {
             throw new ArgumentException("Email format is invalid.", nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value;
    
}