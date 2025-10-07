using Ardalis.Result;
using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Application.Contracts
{
    public interface IBibliotecaService
    {
        Task<Result<BibliotecaDTO>> ObterBibliotecaAsync(int usuarioId);
    }
}
