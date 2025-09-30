using Ardalis.Result;
using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Application.Contracts
{
    public interface IJogoService : IDisposable
    {
        Task<IEnumerable<JogoDTO>> BuscarAsync(string termo, int inicio, int tamanho);
        Task<Result<JogoDTO>> ObterPorIdAsync(Guid id);
        Task<Result<JogoDTO>> CadastrarAsync(CadastrarJogoDTO dto);
        Task<Result<JogoDTO>> AlterarDadosAsync(AlterarDadosDTO dto);
        Task<Result<JogoDTO>> AlterarPrecoAsync(AlterarPrecoDTO dto);
        Task<Result<JogoDTO>> AplicarDescontoAsync(AplicarDescontoDTO dto);
        Task<Result> DeletarAsync(Guid id);
    }
}
