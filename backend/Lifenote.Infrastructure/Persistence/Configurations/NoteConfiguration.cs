using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Note");
        builder.HasKey(x => x.Id).HasName("Note_pkey");
        builder.HasIndex(x => x.UserId, "idx_note_userid");
        builder.HasIndex(x => new { x.UserId, x.CreatedAt }, "idx_note_userid_created");
        builder.Property(x => x.Title).HasMaxLength(200);

        // Tags is a native PostgreSQL text[] column — Npgsql maps string[] natively.
        builder.Property(x => x.Tags)
            .HasColumnType("text[]")
            .HasDefaultValueSql("ARRAY[]::text[]");

        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
