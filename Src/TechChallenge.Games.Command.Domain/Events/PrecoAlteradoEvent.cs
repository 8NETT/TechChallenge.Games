namespace TechChallenge.Games.Command.Domain.Events
{
    public sealed class PrecoAlteradoEvent : BaseEvent
    {
        public required decimal NovoPreco { get; init; }

        public PrecoAlteradoEvent() : base(nameof(PrecoAlteradoEvent)) { }
    }
}
