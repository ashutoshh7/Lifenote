using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class HabitStreakConfiguration : IEntityTypeConfiguration<HabitStreak>
{
    public void Configure(EntityTypeBuilder<HabitStreak> builder)
    {
        builder.ToTable("HabitStreaks");
        builder.HasKey(x => x.Id).HasName("HabitStreaks_pkey");

        builder.HasIndex(x => x.HabitId, "IX_HabitStreaks_HabitId_Unique").IsUnique();
        builder.HasIndex(x => x.UserId, "IX_HabitStreaks_UserId");

        builder.Property(x => x.CurrentStreak).HasDefaultValue(0);
        builder.Property(x => x.LongestStreak).HasDefaultValue(0);
        builder.Property(x => x.TotalCompletions).HasDefaultValue(0);
        builder.Property(x => x.CalculatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // HabitStreaks DB table has no UpdatedAt or CreatedAt columns
        builder.Ignore(x => x.UpdatedAt);
        builder.Ignore(x => x.CreatedAt);

        builder.HasOne(x => x.Habit)
               .WithOne(h => h.Streak)
               .HasForeignKey<HabitStreak>(x => x.HabitId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_HabitStreaks_Habits_HabitId");
    }
}
