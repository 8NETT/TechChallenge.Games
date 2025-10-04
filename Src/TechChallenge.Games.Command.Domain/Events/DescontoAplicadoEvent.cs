namespace TechChallenge.Games.Command.Domain.Events
{
    public sealed class DescontoAplicadoEvent : BaseEvent
    {
        public required int Percentual { get; init; }

        public DescontoAplicadoEvent() : base(nameof(DescontoAplicadoEvent)) { }
    }
}
