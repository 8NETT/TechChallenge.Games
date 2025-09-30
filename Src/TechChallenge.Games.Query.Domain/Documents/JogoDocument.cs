namespace TechChallenge.Games.Query.Domain.Documents
{
    public sealed class JogoDocument
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataLancamento { get; set; }
        public decimal Preco { get; set; }
        public int Desconto { get; set; }
        public decimal Valor { get; set; }
    }
}
