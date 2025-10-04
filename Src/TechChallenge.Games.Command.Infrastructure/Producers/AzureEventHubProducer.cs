using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Command.Infrastructure.Producers
{
    public sealed class AzureEventHubProducer : IJogoProducer, IAsyncDisposable
    {
        private readonly EventHubProducerClient _client;

        public AzureEventHubProducer(string connectionString)
            : this(new EventHubProducerClient(connectionString)) { }

        public AzureEventHubProducer(string connectionString, string eventHubName)
            : this(new EventHubProducerClient(connectionString, eventHubName)) { }

        public AzureEventHubProducer(EventHubProducerClient client)
        {
            _client = client;
        }

        public async Task ProduceAsync(JogoDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);

            using var batch = await _client.CreateBatchAsync();
            batch.TryAdd(new EventData(json));

            await _client.SendAsync(batch);
        }

        public async ValueTask DisposeAsync() =>
            await _client.DisposeAsync();
    }
}
