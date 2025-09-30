using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using TechChallenge.Games.Command.Domain.Events;
using TechChallenge.Games.Command.Domain.Persistence;
using Microsoft.Extensions.Options;

namespace TechChallenge.Games.Command.Infrastructure.Persistence
{
    public sealed class CosmosEventStore : IEventStore
    {
        private readonly CosmosClient _client;
        private readonly Container _container;

        public CosmosEventStore(CosmosConfig config)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var options = new CosmosClientOptions
            {
                Serializer = new NewtonsoftJsonCosmosSerializer(jsonSettings)
            };

            _client = new CosmosClient(config.EndpointUri, config.Key, options);
            _container = _client.GetContainer(config.DatabaseId, config.ContainerId);
        }

        public CosmosEventStore(IOptions<CosmosConfig> options) : this(options.Value) { }

        public async Task AppendAsync(BaseEvent @event)
        {
            await _container.CreateItemAsync(@event, new PartitionKey(@event.AggregateId.ToString()));
        }

        public async Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.aggregateId = @aggregateId ORDER BY c.Timestamp")
                .WithParameter("@aggregateId", aggregateId.ToString());
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(aggregateId.ToString())
            };

            var iterator = _container.GetItemQueryIterator<BaseEvent>(query, requestOptions: requestOptions);
            var events = new List<BaseEvent>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                    events.Add(item);
            }

            return events;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
