using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class FocusSessionConfiguration : IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> builder)
    {
        builder.ToTable("FocusSession");
        builder.HasKey(x => x.Id).HasName("FocusSession_pkey");
        builder.HasIndex(x => x.UserId, "idx_focussessions_userid");
        builder.HasIndex(x => x.StartedAt, "idx_focussessions_starttime");
        builder.HasIndex(x => new { x.UserId, x.StartedAt }, "idx_focussessions_user_date");
        builder.Property(x => x.Id).HasDefaultValueSql("nextval('\"FocusSession_Id_seq\"'::regclass)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
