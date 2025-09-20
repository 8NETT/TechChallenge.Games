using Ardalis.Result;
using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Application.Contracts
{
    public interface IJogoService : IDisposable
    {
        Task<IEnumerable<JogoDTO>> ObterTodosAsync();
        Task<Result<JogoDTO>> ObterPorIdAsync(int id);
        Task<Result<JogoDTO>> CadastrarAsync(CadastrarJogoDTO dto);
        Task<Result<JogoDTO>> AlterarAsync(AlterarJogoDTO dto);
        Task<Result> DeletarAsync(int id);
    }
}
