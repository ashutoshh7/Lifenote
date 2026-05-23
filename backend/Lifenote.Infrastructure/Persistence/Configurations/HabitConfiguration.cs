using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits");
        builder.HasKey(x => x.Id).HasName("Habits_pkey");
        builder.HasIndex(x => x.StartDate, "IX_Habits_StartDate");
        builder.HasIndex(x => new { x.UserId, x.IsActive }, "IX_Habits_UserId_IsActive");
        builder.HasIndex(x => new { x.UserId, x.IsActive }, "idx_habits_active").HasFilter("(\"IsActive\" = true)");
        builder.HasIndex(x => x.CreatedAt, "idx_habits_created");
        builder.HasIndex(x => x.UserId, "idx_habits_userid");
        builder.Property(x => x.Color).HasMaxLength(7).HasDefaultValueSql("'#3498db'::character varying");
        builder.Property(x => x.FrequencyType).HasMaxLength(20).HasDefaultValueSql("'Daily'::character varying");
        builder.Property(x => x.FrequencyValue).HasMaxLength(100);
        builder.Property(x => x.IconName).HasMaxLength(50).HasDefaultValueSql("'fa-check'::character varying");
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.StartDate).HasDefaultValueSql("CURRENT_DATE");
        builder.Property(x => x.TargetCount).HasDefaultValue(1);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
