using TechChallenge.Games.Core.Entity;

namespace TechChallenge.Games.Core.Repository;

public interface IJogoRepository : IRepository<Jogo>
{
    Task<Jogo?> ObterPorNomeAsync(string nome);
}