using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence;

public partial class LifenoteDbContext : DbContext
{
    public LifenoteDbContext()
    {
    }

    public LifenoteDbContext(DbContextOptions<LifenoteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FocusSession> FocusSessions { get; set; }
    public virtual DbSet<Habit> Habits { get; set; }
    public virtual DbSet<HabitLog> HabitLogs { get; set; }
    public virtual DbSet<HabitStreak> HabitStreaks { get; set; }
    public virtual DbSet<Note> Notes { get; set; }
    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FocusSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FocusSession_pkey");
            entity.HasIndex(e => new { e.UserId, e.IsCompleted }, "idx_focussessions_completed");
            entity.HasIndex(e => e.Id, "idx_focussessions_noteid").HasFilter("(\"Id\" IS NOT NULL)");
            entity.HasIndex(e => new { e.UserId, e.SessionType }, "idx_focussessions_sessiontype");
            entity.HasIndex(e => e.StartTime, "idx_focussessions_starttime");
            entity.HasIndex(e => new { e.UserId, e.StartTime }, "idx_focussessions_user_date");
            entity.HasIndex(e => e.UserId, "idx_focussessions_userid");
            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"FocusSession_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.SessionType).HasMaxLength(20);
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Habits_pkey");
            entity.HasIndex(e => e.StartDate, "IX_Habits_StartDate");
            entity.HasIndex(e => new { e.UserId, e.IsActive }, "IX_Habits_UserId_IsActive");
            entity.HasIndex(e => new { e.UserId, e.IsActive }, "idx_habits_active").HasFilter("(\"IsActive\" = true)");
            entity.HasIndex(e => e.CreatedAt, "idx_habits_created");
            entity.HasIndex(e => e.UserId, "idx_habits_userid");
            entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValueSql("'#3498db'::character varying");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FrequencyType).HasMaxLength(20).HasDefaultValueSql("'Daily'::character varying");
            entity.Property(e => e.FrequencyValue).HasMaxLength(100);
            entity.Property(e => e.IconName).HasMaxLength(50).HasDefaultValueSql("'fa-check'::character varying");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.TargetCount).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<HabitLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("HabitLogs_pkey");
            entity.HasIndex(e => new { e.HabitId, e.CompletedDate }, "IX_HabitLogs_HabitId_Date");
            entity.HasIndex(e => new { e.UserId, e.CompletedDate }, "IX_HabitLogs_UserId_Date");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasOne(d => d.Habit).WithMany(p => p.HabitLogs).HasForeignKey(d => d.HabitId);
            entity.HasOne(d => d.User).WithMany(p => p.HabitLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HabitLogs_Users_UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
