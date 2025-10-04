namespace TechChallenge.Games.Command.Domain.Events
{
    public abstract class BaseEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public required Guid AggregateId { get; init; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public string Type { get; }

        protected internal BaseEvent(string type)
        {
            Type = type;
        }
    }
}
