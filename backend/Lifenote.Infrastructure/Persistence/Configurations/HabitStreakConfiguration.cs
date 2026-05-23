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
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasOne(d => d.Habit)
               .WithOne(p => p.Streak)
               .HasForeignKey<HabitStreak>(d => d.HabitId);
    }
}
