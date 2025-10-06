using TechChallenge.Games.Command.Domain.Aggregates;

namespace TechChallenge.Games.Command.Domain.Persistence
{
    public interface IJogoCommandRepository : IDisposable
    {
        public Task<Jogo?> ObterPorIdAsync(Guid id);

        public Task SalvarAsync(Jogo aggregate);
    }
}
