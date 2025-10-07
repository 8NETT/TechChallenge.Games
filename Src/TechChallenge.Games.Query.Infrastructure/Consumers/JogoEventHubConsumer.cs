using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Query.Domain.Persistence;
using TechChallenge.Games.Query.Infrastructure.Mappers;

namespace TechChallenge.Games.Query.Infrastructure.Consumers
{
    public sealed class JogoEventHubConsumer : IHostedService, IAsyncDisposable
    {
        private readonly EventHubConsumerClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private CancellationTokenSource? _cts;

        public JogoEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, ILogger logger = null) 
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString), logger) { }

        public JogoEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, string eventHubName, ILogger logger = null)
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString, eventHubName), logger) { }

        public JogoEventHubConsumer(IServiceProvider serviceProvider, EventHubConsumerClient client, ILogger logger = null)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = Task.Run(async () =>
            {
                await foreach (var @event in _client.ReadEventsAsync(cancellationToken))
                {
                    if (@event.Data == null)
                    {
                        _logger?.LogWarning("Dados do evento de atualização de jogos veio vazio.");
                        continue;
                    }

                    var json = Encoding.UTF8.GetString(@event.Data.EventBody.ToArray());
                    var dto = JsonConvert.DeserializeObject<JogoDTO>(json);

                    if (dto == null)
                    {
                        _logger?.LogWarning("Dados do evento de atualização de jogos veio vazio.");
                        continue;
                    }

                    var document = dto.ToDocument();
                    using var scope = _serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IJogoQueryRepository>();

                    await repository.UpsertAsync(document);

                    _logger?.LogInformation($"Jogo {document.Id} atualizado na base de leitura.");
                }
            }, _cts.Token);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts?.Cancel();
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync() =>
            await _client.DisposeAsync();
    }
}
