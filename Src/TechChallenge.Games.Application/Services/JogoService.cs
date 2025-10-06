using Ardalis.Result;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Application.Mappers;
using TechChallenge.Games.Command.Domain.Persistence;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Application.Services
{
    public class JogoService : BaseService, IJogoService
    {
        private IJogoCommandRepository _commandRepository;
        private IJogoQueryRepository _queryRepository;
        private IJogoProducer _producer;

        public JogoService(
            IJogoCommandRepository commandRepository, 
            IJogoQueryRepository queryRepository,
            IJogoProducer producer)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _producer = producer;
        }

        public async Task<IEnumerable<JogoDTO>> ObterTodosAsync(int inicio, int tamanho)
        {
            var jogos = await _queryRepository.ObterTodosAsync(inicio, tamanho);
            return jogos.Select(j => j.ToDTO());
        }

        public async Task<Result<IEnumerable<JogoDTO>>> BuscarAsync(BuscarJogoDTO dto)
        {
            if (!TryValidate(dto, out var validationResult))
                return validationResult;

            var documents = await _queryRepository.BuscarAsync(dto.Termo, dto.Inicio, dto.Tamanho);

            if (documents == null)
                return Array.Empty<JogoDTO>();

            return documents.Select(d => d.ToDTO()).ToArray();
        }

        public async Task<Result<JogoDTO>> ObterPorIdAsync(Guid id)
        {
            var jogo = await _queryRepository.ObterPorIdAsync(id);

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

            var jogo = dto.ToEntity();
            await _commandRepository.SalvarAsync(jogo);

            var result = jogo.ToDTO();
            await _producer.ProduceAsync(result);

            return result;
        }

        public async Task<Result<JogoDTO>> AlterarDadosAsync(AlterarDadosDTO dto)
        {
            if (!TryValidate(dto, out var validationResult))
                return validationResult;

            var jogo = await _commandRepository.ObterPorIdAsync(dto.Id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");
            if (jogo.Nome != dto.Nome && await ExisteJogoComNomeAsync(dto.Nome))
                return Result.Conflict("Já existe um jogo cadastrado com esse nome.");

            jogo.AlterarDados(dto.Nome, dto.Descricao, dto.DataLancamento);
            await _commandRepository.SalvarAsync(jogo);

            var result = jogo.ToDTO();
            await _producer.ProduceAsync(result);

            return result;
        }

        public async Task<Result<JogoDTO>> AlterarPrecoAsync(AlterarPrecoDTO dto)
        {
            var jogo = await _commandRepository.ObterPorIdAsync(dto.Id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");

            jogo.AlterarPreco(dto.NovoPreco);
            await _commandRepository.SalvarAsync(jogo);

            var result = jogo.ToDTO();
            await _producer.ProduceAsync(result);

            return result;
        }

        public async Task<Result<JogoDTO>> AplicarDescontoAsync(AplicarDescontoDTO dto)
        {
            var jogo = await _commandRepository.ObterPorIdAsync(dto.Id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");

            jogo.AplicarDesconto(dto.Desconto);
            await _commandRepository.SalvarAsync(jogo);

            var result = jogo.ToDTO();
            await _producer.ProduceAsync(result);

            return result;
        }

        public async Task<Result> DeletarAsync(Guid id)
        {
            var jogo = await _commandRepository.ObterPorIdAsync(id);

            if (jogo == null)
                return Result.NotFound("Jogo não localizado.");

            jogo.Remover();
            await _commandRepository.SalvarAsync(jogo);
            await _producer.ProduceAsync(jogo.ToDTO());

            return Result.Success();
        }

        public void Dispose() => _commandRepository.Dispose();

        private async Task<bool> ExisteJogoComNomeAsync(string nome) =>
            await _queryRepository.ObterPorNomeAsync(nome) != null;
    }
}
