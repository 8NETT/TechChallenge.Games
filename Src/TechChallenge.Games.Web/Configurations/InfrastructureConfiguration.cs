using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using TechChallenge.Games.Core.Repository;
using TechChallenge.Games.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Infrastructure.Search;

namespace TechChallenge.Games.Web.Configurations
{
    public static class InfrastructureConfiguration
    {
        public static void AddInfrastructureConfiguration(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;
            var connectionString = config.GetConnectionString("ConnectionString")
                                   ?? throw new InvalidOperationException(
                                       "ConnectionString não localizada no arquivo de configuração.");

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => { options.UseSqlServer(connectionString); }, ServiceLifetime.Scoped);

            if (builder.Environment.IsDevelopment())
            {
                var settings = new ElasticsearchClientSettings(new Uri(config["Elasticsearch:Url"] ?? string.Empty));
                builder.Services.AddSingleton(new ElasticsearchClient(settings));
            }
            else
            {
                var cloudId = builder.Configuration["Elasticsearch:CloudId"];
                var apiKey = builder.Configuration["Elasticsearch:ApiKey"];

                var settings =
                    new ElasticsearchClientSettings(cloudId ?? string.Empty, new ApiKey(apiKey ?? string.Empty));
                builder.Services.AddSingleton<ElasticsearchClient>(new ElasticsearchClient(settings));
            }


            builder.Services.AddScoped<IJogoRepository, JogoRepository>();
            builder.Services.AddScoped<IJogoSearch, JogoSearch>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}