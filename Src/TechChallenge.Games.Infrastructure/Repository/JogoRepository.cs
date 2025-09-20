using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Games.Infrastructure.Repository;

public class JogoRepository : EFRepository<Jogo>, IJogoRepository
{
    public JogoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Jogo?> ObterPorNomeAsync(string nome) =>
        await _dbSet.SingleOrDefaultAsync(j => j.Nome == nome);
}