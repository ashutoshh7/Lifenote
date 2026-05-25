using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lifenote.Infrastructure.Persistence.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.ToTable("Milestones");
        builder.HasKey(x => x.Id).HasName("Milestones_pkey");

        builder.HasIndex(x => x.GoalId, "idx_milestones_goalid");

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.IsCompleted).HasDefaultValue(false);

        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationship to Goal
        builder.HasOne(x => x.Goal)
            .WithMany(g => g.Milestones)
            .HasForeignKey(x => x.GoalId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Milestones_Goals_GoalId");
    }
}
