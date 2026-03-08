using TrainingNutrition.Domain.Common;

namespace TrainingNutrition.Domain.Users;

public sealed class User
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public Email Email { get; private set; }

    public User(Guid id, string name, Email email)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(id));

        Name = ValidateName(name);
        Email = email ?? throw new ArgumentNullException(nameof(email));

        Id = id;
    }

    public void ChangeName(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangeEmail(Email email)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be empty.", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("User name cannot be longer than 100 characters.", nameof(name));

        return name.Trim();
    }

}
