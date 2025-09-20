using Ardalis.Result;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Application.Mappers;
using TechChallenge.Games.Core.Repository;

namespace TechChallenge.Games.Application.Services
{
    public class JogoService(IUnitOfWork unitOfWork) : BaseService, IJogoService
    {
        public async Task<IEnumerable<JogoDTO>> ObterTodosAsync()
        {
            var jogos = await unitOfWork.JogoRepository.ObterTodosAsync();
            return jogos.Select(j => j.ToDTO());
        }

        public async Task<Result<JogoDTO>> ObterPorIdAsync(int id)
        {
            var jogo = await unitOfWork.JogoRepository.ObterPorIdAsync(id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");

            return jogo.ToDTO();
        }

        public async Task<Result<JogoDTO>> CadastrarAsync(CadastrarJogoDTO dto)
        {
            if (!TryValidate(dto, out var validationResult))
                return validationResult;

            if (await ExisteJogoComNomeAsync(dto.Nome))
                return Result.Conflict("Já existe um jogo cadastrado com esse nome.");

            var entidade = dto.ToEntity();

            unitOfWork.JogoRepository.Cadastrar(entidade);
            await unitOfWork.CommitAsync();

            return entidade.ToDTO();
        }

        public async Task<Result<JogoDTO>> AlterarAsync(AlterarJogoDTO dto)
        {
            if (!TryValidate(dto, out var validationResult))
                return validationResult;

            var jogo = await unitOfWork.JogoRepository.ObterPorIdAsync(dto.Id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");
            if (jogo.Nome != dto.Nome && await ExisteJogoComNomeAsync(dto.Nome))
                return Result.Conflict("Já existe um jogo cadastrado com esse nome.");

            var entidade = dto.ToEntity(jogo);

            unitOfWork.JogoRepository.Cadastrar(entidade);
            await unitOfWork.CommitAsync();

            return entidade.ToDTO();
        }

        public async Task<Result> DeletarAsync(int id)
        {
            var jogo = await unitOfWork.JogoRepository.ObterPorIdAsync(id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");

            unitOfWork.JogoRepository.Deletar(jogo);
            await unitOfWork.CommitAsync();

            return Result.Success();
        }

        public void Dispose() => unitOfWork.Dispose();

        private async Task<bool> ExisteJogoComNomeAsync(string nome) =>
            await unitOfWork.JogoRepository.ObterPorNomeAsync(nome) != null;
    }
}
