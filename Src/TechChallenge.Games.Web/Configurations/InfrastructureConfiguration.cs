using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using TechChallenge.Games.Command.Infrastructure.Persistence;
using TechChallenge.Games.Command.Domain.Persistence;
using TechChallenge.Games.Query.Domain.Persistence;
using TechChallenge.Games.Query.Infrastructure.Persistence;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Command.Infrastructure.Producers;
using TechChallenge.Games.Query.Infrastructure.Consumers;

namespace TechChallenge.Games.Web.Configurations
{
    public static class InfrastructureConfiguration
    {
        public static void AddInfrastructureConfiguration(this WebApplicationBuilder builder)
        {
            builder.AddEventStoreConfiguration();
            builder.AddElasticsearchConfiguration();
            builder.AddQueryRepositoryConfiguration();
            builder.AddProducerConfiguration();
            builder.AddConsumerConfiguration();
        }

        public static void AddEventStoreConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<CosmosConfig>(builder.Configuration.GetSection(nameof(CosmosConfig)));
            builder.Services.AddScoped<IEventStore, CosmosEventStore>();
            builder.Services.AddScoped<IJogoCommandRepository, JogoCommandRepository>();
        }

        public static void AddElasticsearchConfiguration(this WebApplicationBuilder builder)
        {
            var cloudId = builder.Configuration["Elasticsearch:CloudId"];
            var apiKey = builder.Configuration["Elasticsearch:ApiKey"];

            if (string.IsNullOrWhiteSpace(cloudId) || string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Cloud ID e/ou API Key do Elasticsearch não localizado no arquivo de configuração.");

            var settings = new ElasticsearchClientSettings(new Uri(cloudId))
                .Authentication(new ApiKey(apiKey));
            builder.Services.AddSingleton(new ElasticsearchClient(settings));
        }

        public static void AddQueryRepositoryConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IJogoQueryRepository, JogoQueryRepository>(f =>
            {
                var client = f.GetRequiredService<ElasticsearchClient>();
                var indexName = builder.Configuration["Elasticsearch:IndexName"];

                if (string.IsNullOrWhiteSpace(indexName))
                    throw new InvalidOperationException("Nome do índice do Elasticsearch não localizado no arquivo de configuração.");

                return new JogoQueryRepository(client, indexName);
            });
        }

        public static void AddProducerConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IJogoProducer, AzureEventHubProducer>(f =>
            {
                var connectionString = builder.Configuration["AzureEventHub:ConnectionString"];
                var eventHubName = builder.Configuration["AzureEventHub:HubName"];

                if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(eventHubName))
                    throw new InvalidOperationException("Configuração do hub de eventos não localizado no arquivo de configuração.");

                return new AzureEventHubProducer(connectionString, eventHubName);
            });
        }

        public static void AddConsumerConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService(s =>
            {
                var connectionString = builder.Configuration["JogoEventHub:ConnectionString"];
                var eventHubName = builder.Configuration["JogoEventHub:HubName"];
                var consumerGroup = builder.Configuration["JogoEventHub:ConsumerGroup"];

                if (string.IsNullOrWhiteSpace(connectionString) ||
                    string.IsNullOrWhiteSpace(eventHubName) ||
                    string.IsNullOrWhiteSpace(consumerGroup))
                    throw new InvalidOperationException("Configuração do hub de jogos não localizado no arquivo de configuração.");

                return new JogoEventHubConsumer(s, consumerGroup, connectionString, eventHubName);
            });

            builder.Services.AddHostedService(s =>
            {
                var connectionString = builder.Configuration["BibliotecaEventHub:ConnectionString"];
                var eventHubName = builder.Configuration["BibliotecaEventHub:HubName"];
                var consumerGroup = builder.Configuration["BibliotecaEventHub:ConsumerGroup"];

                if (string.IsNullOrWhiteSpace(connectionString) ||
                    string.IsNullOrWhiteSpace(eventHubName) ||
                    string.IsNullOrWhiteSpace(consumerGroup))
                    throw new InvalidOperationException("Configuração do hub de biblioteca não localizado no arquivo de configuração.");

                return new BibliotecaEventHubConsumer(s, consumerGroup, connectionString, eventHubName);
            });
        }
    }
}