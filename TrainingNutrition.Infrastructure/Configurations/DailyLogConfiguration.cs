using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingNutrition.Domain.Meals;
using TrainingNutrition.Domain.Tracking;

namespace TrainingNutrition.Infrastructure.Configurations;

public class DailyLogConfiguration : IEntityTypeConfiguration<DailyLog>
{
    public void Configure(EntityTypeBuilder<DailyLog> builder)
    {
          // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.HasMany(x => x.Meals)
            .WithOne()
            .HasForeignKey("DailyLogId")
            .IsRequired();


    }
}