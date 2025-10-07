using Ardalis.Result;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Application.Mappers;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Application.Services
{
    public sealed class BibliotecaService : IBibliotecaService
    {
        private readonly IBibliotecaQueryRepository _bibliotecaRepository;
        private readonly IJogoQueryRepository _jogoRepository;

        public BibliotecaService(IBibliotecaQueryRepository bibliotecaRepository, IJogoQueryRepository jogoRepository)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _jogoRepository = jogoRepository;
        }

        public async Task<Result<BibliotecaDTO>> ObterBibliotecaAsync(int usuarioId)
        {
            var biblioteca = await _bibliotecaRepository.ObterPorUsuarioIdAsync(usuarioId);

            if (biblioteca == null)
                return Result.NotFound("Biblioteca não localizada.");

            var jogos = new List<JogoDTO>();

            foreach (var jogoId in biblioteca.Jogos)
            {
                var jogo = await _jogoRepository.ObterPorIdAsync(jogoId);
                if (jogo != null)
                    jogos.Add(jogo.ToDTO());
            }

            var dto = new BibliotecaDTO
            {
                UsuarioId = biblioteca.UsuarioId,
                Jogos = jogos
            };

            return dto;
        }
    }
}
