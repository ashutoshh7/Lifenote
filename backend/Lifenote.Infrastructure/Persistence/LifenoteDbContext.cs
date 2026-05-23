using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for Lifenote.
/// Model configuration is based on the actual Domain entity properties.
/// Navigation properties and columns that no longer exist in Domain are omitted.
/// </summary>
public partial class LifenoteDbContext : DbContext
{
    public LifenoteDbContext() { }

    public LifenoteDbContext(DbContextOptions<LifenoteDbContext> options)
        : base(options) { }

    public virtual DbSet<FocusSession> FocusSessions { get; set; }
    public virtual DbSet<Habit> Habits { get; set; }
    public virtual DbSet<HabitLog> HabitLogs { get; set; }
    public virtual DbSet<HabitStreak> HabitStreaks { get; set; }
    public virtual DbSet<Note> Notes { get; set; }
    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ------- FocusSession -------
        // Domain entity has: UserId, StartedAt, EndedAt, DurationMinutes, Notes
        // (IsCompleted, SessionType, StartTime from the old Core model are gone)
        modelBuilder.Entity<FocusSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FocusSession_pkey");
            entity.HasIndex(e => e.UserId, "idx_focussessions_userid");
            entity.HasIndex(e => e.StartedAt, "idx_focussessions_starttime");
            entity.HasIndex(e => new { e.UserId, e.StartedAt }, "idx_focussessions_user_date");
            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"FocusSession_Id_seq\"'::regclass)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ------- Habit -------
        // Domain entity has: UserId, Name, Description, Color, IconName,
        //                    FrequencyType, FrequencyValue, TargetCount, IsActive, StartDate, EndDate
        // Navigation: Logs (ICollection<HabitLog>), Streak (HabitStreak?)
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

        // ------- HabitLog -------
        // Domain entity has: HabitId, UserId, CompletedAt, CompletedDate, Notes
        // Navigation: Habit? (back-ref to aggregate root)
        // NOTE: Old Core model had a .User navigation property — removed from Domain entity.
        modelBuilder.Entity<HabitLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("HabitLogs_pkey");
            entity.HasIndex(e => new { e.HabitId, e.CompletedDate }, "IX_HabitLogs_HabitId_Date");
            entity.HasIndex(e => new { e.UserId, e.CompletedDate }, "IX_HabitLogs_UserId_Date");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Notes).HasMaxLength(500);
            // Logs -> Habit (aggregate root back-reference via Logs collection on Habit)
            entity.HasOne(d => d.Habit)
                  .WithMany(p => p.Logs)
                  .HasForeignKey(d => d.HabitId);
        });

        // ------- HabitStreak -------
        modelBuilder.Entity<HabitStreak>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HabitId, "idx_habitstreaks_habitid");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ------- Note -------
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Notes_pkey");
            entity.HasIndex(e => e.UserId, "idx_notes_userid");
            entity.HasIndex(e => e.CreatedAt, "idx_notes_created");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ------- UserInfo -------
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserInfo_pkey");
            entity.HasIndex(e => e.FirebaseUid, "idx_userinfo_firebaseuid").IsUnique();
            entity.HasIndex(e => e.Email, "idx_userinfo_email").IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
