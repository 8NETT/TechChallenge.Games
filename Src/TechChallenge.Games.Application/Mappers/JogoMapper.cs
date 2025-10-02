using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Query.Domain.Documents;

namespace TechChallenge.Games.Application.Mappers
{
    internal static class JogoMapper
    {
        public static JogoDTO ToDTO(this Jogo entidade) => new JogoDTO
        {
            Id = entidade.Id,
            Nome = entidade.Nome,
            Descricao = entidade.Descricao,
            DataLancamento = entidade.DataLancamento,
            Preco = entidade.Valor,
            Desconto = entidade.Desconto,
            Valor = entidade.Valor,
            Deletado = entidade.Deletado
        };

        public static JogoDTO ToDTO(this JogoDocument document) => new JogoDTO
        {
            Id = document.Id,
            Nome = document.Nome,
            Descricao = document.Descricao,
            DataLancamento = document.DataLancamento,
            Preco = document.Preco,
            Desconto = document.Desconto,
            Valor = document.Valor,
            Deletado = document.Deletado
        };

        public static Jogo ToEntity(this CadastrarJogoDTO dto) => Jogo.Novo
            .Nome(dto.Nome)
            .Descricao(dto.Descricao)
            .DataLancamento(dto.DataLancamento)
            .Preco(dto.Preco)
            .Build();
    }
}
