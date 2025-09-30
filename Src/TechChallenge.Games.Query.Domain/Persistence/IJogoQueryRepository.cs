using TechChallenge.Games.Query.Domain.Documents;

namespace TechChallenge.Games.Query.Domain.Persistence
{
    public interface IJogoQueryRepository
    {
        Task<JogoDocument?> ObterPorIdAsync(Guid id);
        Task<JogoDocument?> ObterPorNomeAsync(string nome);
        Task<IEnumerable<JogoDocument>> ObterTodosAsync(int inicio, int tamanho);
        Task<IEnumerable<JogoDocument>> BuscarAsync(string query, int inicio, int tamanho);
        Task<bool> UpsertAsync(JogoDocument jogo);
    }
}
