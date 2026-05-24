using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class FocusSessionConfiguration : IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> builder)
    {
        builder.ToTable("FocusSessions");
        builder.HasKey(x => x.Id).HasName("FocusSession_pkey");

        builder.HasIndex(x => x.UserId, "idx_focussessions_userid");
        builder.HasIndex(x => x.StartTime, "idx_focussessions_starttime");
        builder.HasIndex(x => new { x.UserId, x.StartTime }, "idx_focussessions_user_date");
        builder.HasIndex(x => new { x.UserId, x.IsCompleted }, "idx_focussessions_completed");
        builder.HasIndex(x => new { x.UserId, x.SessionType }, "idx_focussessions_sessiontype");

        builder.Property(x => x.SessionType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Duration).IsRequired();
        builder.Property(x => x.IsCompleted).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // FocusSessions DB table has no UpdatedAt column
        builder.Ignore(x => x.UpdatedAt);
    }
}
