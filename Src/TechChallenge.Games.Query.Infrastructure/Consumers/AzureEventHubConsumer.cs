using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Query.Domain.Persistence;
using TechChallenge.Games.Query.Infrastructure.Mappers;

namespace TechChallenge.Games.Query.Infrastructure.Consumers
{
    public sealed class AzureEventHubConsumer : IHostedService, IAsyncDisposable
    {
        private readonly EventHubConsumerClient _client;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _cts;

        public AzureEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString) 
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString)) { }

        public AzureEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, string eventHubName)
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString, eventHubName)) { }

        public AzureEventHubConsumer(IServiceProvider serviceProvider, EventHubConsumerClient client)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await foreach (var @event in _client.ReadEventsAsync(cancellationToken))
            {
                if (@event.Data == null)
                    continue;

                var json = Encoding.UTF8.GetString(@event.Data.EventBody.ToArray());
                var dto = JsonConvert.DeserializeObject<JogoDTO>(json);

                if (dto == null)
                    continue;

                var document = dto.ToDocument();
                using var scope = _serviceProvider.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IJogoQueryRepository>();

                await repository.UpsertAsync(document);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_cts == null)
                return;

            await _cts.CancelAsync();
        }

        public async ValueTask DisposeAsync() =>
            await _client.DisposeAsync();
    }
}
