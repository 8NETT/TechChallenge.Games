using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Query.Domain.Documents;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Query.Infrastructure.Consumers
{
    public sealed class BibliotecaEventHubConsumer : IHostedService, IAsyncDisposable
    {
        private readonly EventHubConsumerClient _client;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource? _cts;

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString) 
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString)) { }

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, string eventHubName)
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString, eventHubName)) { }

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, EventHubConsumerClient client)
        {
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = Task.Run(async () =>
            {
                await foreach (var @event in _client.ReadEventsAsync(cancellationToken))
                {
                    if (@event.Data == null)
                        continue;

                    var json = Encoding.UTF8.GetString(@event.Data.EventBody.ToArray());
                    var dto = JsonConvert.DeserializeObject<PagamentoDTO>(json);

                    if (dto == null)
                        continue;

                    if (dto.Status != 0) // Ignora pagamentos que não foram aprovados
                        continue;

                    using var scope = _serviceProvider.CreateScope();
                    var bibliotecaRepository = scope.ServiceProvider.GetRequiredService<IBibliotecaQueryRepository>();
                    var jogoRepository = scope.ServiceProvider.GetRequiredService<IJogoQueryRepository>();

                    var biblioteca = await bibliotecaRepository.ObterPorUsuarioIdAsync(dto.UserId);
                    if (biblioteca == null)
                        biblioteca = new BibliotecaDocument { UsuarioId = dto.UserId };

                    var jogo = await jogoRepository.ObterPorIdAsync(dto.JogoId);
                    if (jogo == null)
                        continue;

                    if (biblioteca.Jogos.Contains(jogo.Id)) // se o jogo já existe na biblioteca, ignora
                        continue;

                    biblioteca.Jogos.Add(jogo.Id);

                    await bibliotecaRepository.UpsertAsync(biblioteca);
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
