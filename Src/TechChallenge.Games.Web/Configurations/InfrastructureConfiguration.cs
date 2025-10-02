using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using TechChallenge.Games.Command.Infrastructure.Persistence;
using TechChallenge.Games.Command.Domain.Persistence;
using TechChallenge.Games.Query.Domain.Persistence;
using TechChallenge.Games.Query.Infrastructure.Persistence;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Command.Infrastructure.Producers;
using Microsoft.Extensions.Azure;
using TechChallenge.Games.Query.Infrastructure.Consumers;

namespace TechChallenge.Games.Web.Configurations
{
    public static class InfrastructureConfiguration
    {
        public static void AddInfrastructureConfiguration(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;
            builder.Services.Configure<CosmosConfig>(config.GetSection(nameof(CosmosConfig)));
            builder.Services.AddScoped<IEventStore, CosmosEventStore>();

            if (builder.Environment.IsDevelopment())
            {
                var username = config["Elasticsearch:Username"];
                var password = config["Elasticsearch:Password"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new InvalidOperationException(
                        "Username ou Password do Elasticsearch não localizado no arquivo de configuração.");

                var settings = new ElasticsearchClientSettings(new Uri(config["Elasticsearch:Url"] ?? string.Empty))
                    .Authentication(new BasicAuthentication(username, password));

                builder.Services.AddSingleton(new ElasticsearchClient(settings));
            }
            else
            {
                var cloudId = builder.Configuration["Elasticsearch:CloudId"];
                var apiKey = builder.Configuration["Elasticsearch:ApiKey"];

                var settings =
                    new ElasticsearchClientSettings(cloudId ?? string.Empty, new ApiKey(apiKey ?? string.Empty));
                builder.Services.AddSingleton(new ElasticsearchClient(settings));
            }

            builder.Services.AddScoped<IJogoQueryRepository, JogoQueryRepository>(f =>
            {
                var client = f.GetRequiredService<ElasticsearchClient>();
                var indexName = builder.Configuration["Elasticsearch:IndexName"];

                if (string.IsNullOrWhiteSpace(indexName))
                    throw new InvalidOperationException("Nome do índice do Elasticsearch não localizado no arquivo de configuração.");

                return new JogoQueryRepository(client, indexName);
            });

            builder.Services.AddScoped<IJogoProducer, AzureEventHubProducer>(f =>
            {
                var connectionString = builder.Configuration["AzureEventHub:ConnectionString"];
                var eventHubName = builder.Configuration["AzureEventHub:HubName"];

                if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(eventHubName))
                    throw new InvalidOperationException("Configuração do hub de eventos não localizado no arquivo de configuração.");

                return new AzureEventHubProducer(connectionString, eventHubName);
            });

            builder.Services.AddHostedService(s =>
            {
                var connectionString = builder.Configuration["AzureEventHub:ConnectionString"];
                var eventHubName = builder.Configuration["AzureEventHub:HubName"];
                var consumerGroup = builder.Configuration["AzureEventHub:ConsumerGroup"];

                if (string.IsNullOrWhiteSpace(connectionString) ||
                    string.IsNullOrWhiteSpace(eventHubName) ||
                    string.IsNullOrWhiteSpace(consumerGroup))
                    throw new InvalidOperationException("Configuração do hub de eventos não localizado no arquivo de configuração.");

                return new AzureEventHubConsumer(s, consumerGroup, connectionString, eventHubName);
            });
        }
    }
}