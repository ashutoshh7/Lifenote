using Lifenote.Core.Interfaces;

namespace Lifenote.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LifenoteDbContext _context;
        public IUserInfoRepository Users { get; private set; }
        public INoteRepository Notes { get; private set; }
        public IHabitRepository Habits { get; private set; }
        public IHabitStreakRepository HabitStreak { get; private set; }

        public UnitOfWork(LifenoteDbContext context)
        {
            _context = context;
            Users = new UserInfoRepository(_context);
            Notes = new NoteRepository(_context);
            Habits = new HabitRepository(_context);
            HabitStreak = new HabitStreakRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
