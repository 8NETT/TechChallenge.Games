using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Command.Domain.Events;

namespace TechChallenge.Games.Command.Domain.Exceptions
{
    public sealed class EventoInvalidoException<T> : DomainException where T : BaseAggregate
    {
        private readonly BaseEvent _event;

        public EventoInvalidoException(BaseEvent @event) : base($"Evento do tipo {@event.GetType().Name} inválido para aggregate {nameof(T)}.")
        {
            _event = @event;
        }
    }
}
