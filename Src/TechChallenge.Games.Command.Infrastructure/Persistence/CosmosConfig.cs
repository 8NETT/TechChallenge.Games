namespace TechChallenge.Games.Command.Infrastructure.Persistence
{
    public sealed class CosmosConfig
    {
        public required string EndpointUri { get; init; }
        public required string Key { get; init; }
        public required string ContainerId { get; init; }
        public required string DatabaseId { get; init; }
    }
}
