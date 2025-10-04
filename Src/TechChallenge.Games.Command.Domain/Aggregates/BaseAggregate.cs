using TechChallenge.Games.Command.Domain.Events;

namespace TechChallenge.Games.Command.Domain.Aggregates
{
    public abstract class BaseAggregate
    {
        private readonly List<BaseEvent> _changes = new();

        public Guid Id { get; internal set; }
        public bool Deletado { get; internal set; }

        public IEnumerable<BaseEvent> GetUncommittedEvents() =>
            _changes;

        public void MarkEventsAsCommitted() =>
            _changes.Clear();

        internal void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
                ApplyChange(@event);
        }

        private void ApplyChange(BaseEvent @event, bool isNew = false)
        {
            Apply(@event);

            if (isNew)
                _changes.Add(@event);
        }
        protected abstract void Apply(BaseEvent @event);

        internal void RaiseEvent(BaseEvent @event) =>
            ApplyChange(@event, true);
    }
}
