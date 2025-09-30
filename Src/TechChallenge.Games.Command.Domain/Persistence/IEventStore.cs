using TechChallenge.Games.Command.Domain.Events;

namespace TechChallenge.Games.Command.Domain.Persistence
{
    public interface IEventStore : IDisposable
    {
        Task AppendAsync(BaseEvent @event);
        Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregateId);
    }
}
