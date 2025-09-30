using TechChallenge.Games.Command.Domain.Aggregates;

namespace TechChallenge.Games.Command.Domain.Persistence
{
    public sealed class JogoRepository : IDisposable
    {
        private readonly IEventStore _eventStore;

        public JogoRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<Jogo?> GetByIdAsync(Guid id)
        {
            var events = await _eventStore.GetEventsAsync(id);

            if (!events.Any())
                return null;

            var aggregate = new Jogo();
            aggregate.ReplayEvents(events);

            return aggregate;
        }

        public async Task SaveAsync(Jogo aggregate)
        {
            var uncommittedEvents = aggregate.GetUncommittedEvents().ToList();
            foreach (var @event in uncommittedEvents)
                await _eventStore.AppendAsync(@event);
            aggregate.MarkEventsAsCommitted();
        }

        public void Dispose() => _eventStore.Dispose();
    }
}
