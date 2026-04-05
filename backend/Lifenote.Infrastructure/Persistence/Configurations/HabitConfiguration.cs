using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Habit — kept out of the Domain entity.
/// </summary>
public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("habits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.FrequencyType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(50);
        builder.Property(x => x.IconName).HasMaxLength(100);
        builder.HasMany(x => x.Logs).WithOne(x => x.Habit).HasForeignKey(x => x.HabitId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Streak).WithOne(x => x.Habit).HasForeignKey<HabitStreak>(x => x.HabitId).OnDelete(DeleteBehavior.Cascade);
    }
}
