using TechChallenge.Games.Query.Domain.Documents;

namespace TechChallenge.Games.Query.Domain.Persistence
{
    public interface IBibliotecaQueryRepository
    {
        Task<BibliotecaDocument?> ObterPorUsuarioIdAsync(int usuarioId);
        Task<bool> UpsertAsync(BibliotecaDocument biblioteca);
    }
}
