namespace TechChallenge.Games.Command.Domain.Events
{
    public sealed class DadosAlteradosEvent : BaseEvent
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataLancamento { get; set; }
        public decimal? Preco { get; set; }

        public DadosAlteradosEvent() : base(nameof(DadosAlteradosEvent)) { }
    }
}
