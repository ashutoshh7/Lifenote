using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class ActiveTimerConfiguration : IEntityTypeConfiguration<ActiveTimer>
{
    public void Configure(EntityTypeBuilder<ActiveTimer> builder)
    {
        builder.ToTable("ActiveTimers");
        builder.HasKey(x => x.Id).HasName("ActiveTimer_pkey");

        // Unique constraint: one row per (UserId, TimerId) pair
        builder.HasIndex(x => new { x.UserId, x.TimerId }, "idx_activetimers_user_timerid")
               .IsUnique();

        builder.HasIndex(x => x.UserId, "idx_activetimers_userid");

        builder.Property(x => x.TimerId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SessionType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(10).IsRequired().HasDefaultValue("idle");
        builder.Property(x => x.TotalDurationSeconds).IsRequired();
        builder.Property(x => x.StartedAt).IsRequired(false);
        builder.Property(x => x.RemainingSeconds).IsRequired(false);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // ActiveTimers has no UpdatedAt column
        builder.Ignore(x => x.UpdatedAt);
    }
}
