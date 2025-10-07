using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private CancellationTokenSource? _cts;

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, ILogger logger = null) 
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString), logger) { }

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, string consumerGroup, string connectionString, string eventHubName, ILogger logger = null)
            : this(serviceProvider, new EventHubConsumerClient(consumerGroup, connectionString, eventHubName), logger) { }

        public BibliotecaEventHubConsumer(IServiceProvider serviceProvider, EventHubConsumerClient client, ILogger logger = null)
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
                        _logger?.LogWarning("Dados dos evento veio vazio.");
                        continue;
                    }

                    var json = Encoding.UTF8.GetString(@event.Data.EventBody.ToArray());

                    PagamentoDTO? dto;
                    try
                    {
                        dto = JsonConvert.DeserializeObject<PagamentoDTO>(json);
                    }
                    catch(JsonException)
                    {
                        continue;
                    }

                    if (dto == null)
                    {
                        _logger?.LogWarning("Dados do evento veio vazio.");
                        continue;
                    }

                    if (dto.Status != 0) // Ignora pagamentos que não foram aprovados
                    {
                        _logger?.LogTrace($"Pagamento {dto.OrderId} ignorado por não estar aprovado.");
                        continue;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var bibliotecaRepository = scope.ServiceProvider.GetRequiredService<IBibliotecaQueryRepository>();
                    var jogoRepository = scope.ServiceProvider.GetRequiredService<IJogoQueryRepository>();

                    var biblioteca = await bibliotecaRepository.ObterPorUsuarioIdAsync(dto.UserId);
                    if (biblioteca == null)
                    {
                        biblioteca = new BibliotecaDocument { UsuarioId = dto.UserId };
                        _logger?.LogTrace($"Nova biblioteca criada para o usuário com ID {dto.UserId}.");
                    }

                    var jogo = await jogoRepository.ObterPorIdAsync(dto.JogoId);
                    if (jogo == null)
                    {
                        _logger?.LogError($"Jogo {dto.JogoId} não localizado para o pagamento {dto.OrderId}.");
                        continue;
                    }

                    if (biblioteca.Jogos.Contains(jogo.Id)) // se o jogo já existe na biblioteca, ignora
                    {
                        _logger?.LogError($"Usuário {dto.UserId} já possui o jogo {dto.JogoId}.");
                        continue;
                    }

                    biblioteca.Jogos.Add(jogo.Id);
                    await bibliotecaRepository.UpsertAsync(biblioteca);

                    _logger?.LogInformation($"Jogo {jogo.Id} adicionado na biblioteca do usuário {biblioteca.UsuarioId}.");
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
