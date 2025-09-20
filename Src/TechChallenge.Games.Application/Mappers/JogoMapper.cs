using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Core.Entity;

namespace TechChallenge.Games.Application.Mappers
{
    internal static class JogoMapper
    {
        public static JogoDTO ToDTO(this Jogo entidade) => new JogoDTO
        {
            Id = entidade.Id,
            Nome = entidade.Nome,
            Valor = entidade.Valor,
            Desconto = entidade.Desconto
        };

        public static Jogo ToEntity(this CadastrarJogoDTO dto) => Jogo.New()
            .DataCriacao(DateTime.Now)
            .Nome(dto.Nome)
            .Valor(dto.Valor)
            .Descricao(dto.Descricao)
            .Desconto(dto.Desconto)
            .Build();

        public static Jogo ToEntity(this AlterarJogoDTO dto, Jogo entidade) => Jogo.New()
            .Id(entidade.Id)
            .DataCriacao(entidade.DataCriacao)
            .Nome(dto.Nome)
            .Valor(dto.Valor)
            .Descricao(dto.Descricao)
            .Desconto(dto.Desconto)
            .Build();
    }
}
