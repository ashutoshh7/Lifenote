using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for Lifenote.
/// Table mapping and column configuration is handled exclusively via
/// IEntityTypeConfiguration&lt;T&gt; classes in the Configurations/ folder.
/// ApplyConfigurationsFromAssembly() discovers and applies them all automatically.
/// </summary>
public partial class LifenoteDbContext : DbContext
{
    public LifenoteDbContext() { }

    public LifenoteDbContext(DbContextOptions<LifenoteDbContext> options)
        : base(options) { }

    public virtual DbSet<ActiveTimer> ActiveTimers { get; set; }
    public virtual DbSet<FocusSession> FocusSessions { get; set; }
    public virtual DbSet<Goal> Goals { get; set; }
    public virtual DbSet<Milestone> Milestones { get; set; }
    public virtual DbSet<Note> Notes { get; set; }
    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Discovers and applies all IEntityTypeConfiguration&lt;T&gt; classes
        // in this assembly (Configurations/HabitConfiguration.cs, UserInfoConfiguration.cs, etc.)
        // This is the single source of truth for table names, indexes, column rules, and relationships.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifenoteDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
