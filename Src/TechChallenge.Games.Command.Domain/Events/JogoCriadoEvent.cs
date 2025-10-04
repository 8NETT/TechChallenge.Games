namespace TechChallenge.Games.Command.Domain.Events
{
    public sealed class JogoCriadoEvent : BaseEvent
    {
        public required string Nome { get; init; }
        public string? Descricao { get; set; }
        public required DateTime DataLancamento { get; init; }
        public required decimal Preco { get; init; }

        public JogoCriadoEvent() : base(nameof(JogoCriadoEvent)) { }
    }
}
