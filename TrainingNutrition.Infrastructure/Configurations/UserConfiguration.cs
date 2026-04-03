using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Common;
using TrainingNutrition.Domain.Users;

namespace TrainingNutrition.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // Name column: required, max length from domain validation, unique
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Email: required, unique — no two accounts can share the same email
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(254); // max length per RFC 5321

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

    }
}