using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class HabitLogConfiguration : IEntityTypeConfiguration<HabitLog>
{
    public void Configure(EntityTypeBuilder<HabitLog> builder)
    {
        builder.ToTable("HabitLogs");
        builder.HasKey(x => x.Id).HasName("HabitLogs_pkey");
        builder.HasIndex(x => new { x.HabitId, x.CompletedDate }, "IX_HabitLogs_HabitId_Date");
        builder.HasIndex(x => new { x.UserId, x.CompletedDate }, "IX_HabitLogs_UserId_Date");
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasOne(d => d.Habit)
               .WithMany(p => p.Logs)
               .HasForeignKey(d => d.HabitId);
    }
}
