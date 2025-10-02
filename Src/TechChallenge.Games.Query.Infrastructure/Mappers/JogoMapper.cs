using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Query.Domain.Documents;

namespace TechChallenge.Games.Query.Infrastructure.Mappers
{
    internal static class JogoMapper
    {
        public static JogoDocument ToDocument(this JogoDTO dto) => new JogoDocument
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            DataLancamento = dto.DataLancamento,
            Preco = dto.Preco,
            Desconto = dto.Desconto,
            Valor = dto.Valor,
            Deletado = dto.Deletado
        };
    }
}
