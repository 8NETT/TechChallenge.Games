using TechChallenge.Games.Core.Repository;

namespace TechChallenge.Games.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        private IJogoRepository _jogoRepository = null!;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IJogoRepository JogoRepository =>
            _jogoRepository = _jogoRepository ?? new JogoRepository(_context);
        
        public async Task CommitAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
