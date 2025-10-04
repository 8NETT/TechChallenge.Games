namespace TechChallenge.Games.Application.DTOs
{
    public class JogoDTO
    {
        public required Guid Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public required DateTime DataLancamento { get; set; }
        public required decimal Preco { get; set; }
        public required int Desconto { get; set; }
        public required decimal Valor { get; set; }
        public bool Deletado { get; set; }
    }
}
