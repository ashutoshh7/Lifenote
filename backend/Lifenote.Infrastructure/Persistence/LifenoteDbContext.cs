using Lifenote.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lifenote.Infrastructure.Persistence;

/// <summary>
/// Moved from Lifenote.Data/Data/LifenoteDbContext.cs.
/// DbContext lives exclusively in Infrastructure — Domain entities are mapped here
/// using IEntityTypeConfiguration classes instead of data annotations on the entity.
/// </summary>
public class LifenoteDbContext : DbContext
{
    public LifenoteDbContext(DbContextOptions<LifenoteDbContext> options) : base(options) { }

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitLog> HabitLogs => Set<HabitLog>();
    public DbSet<HabitStreak> HabitStreaks => Set<HabitStreak>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<UserInfo> Users => Set<UserInfo>();
    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically applies all IEntityTypeConfiguration<T> classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifenoteDbContext).Assembly);
    }
}
