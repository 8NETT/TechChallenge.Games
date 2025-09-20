using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using TechChallenge.Games.Core.Documents;

namespace TechChallenge.Games.Infrastructure.Configurations;

public static class ElasticSetup
{
    public const string IndexName = "jogos";

    public static async Task EnsureJogosIndexAsync(ElasticsearchClient es)
    {
        var exists = await es.Indices.ExistsAsync(IndexName);
        if (exists.Exists) return;

        var response = await es.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("pt_brasil", ca => ca 
                            .Tokenizer("standard")
                            .Filter("lowercase", "asciifolding", "portuguese_stemmer")
                        )
                    )
                    .TokenFilters(tf => tf
                        .Stemmer("portuguese_stemmer", st => st
                            .Language("light_portuguese")
                        )
                    )
                )
            )
            .Mappings(m => m
                .Properties<JogoDocument>(ps => ps
                    .IntegerNumber(p => p.Id)
                    .DoubleNumber(p => p.Valor)
                    .IntegerNumber(p => p.Desconto)
                    .IntegerNumber(p => p.Popularidade)
                    .Date(p => p.DataCriacao)
                    .Keyword(p => p.Keywords)
                    .Text(p => p.Nome, t => t.Analyzer("pt_brasil"))
                    .Text(p => p.Descricao, t => t.Analyzer("pt_brasil"))
                )
            )
        );

        if (!response.IsValidResponse)
        {
            // O response.DebugInformation aqui vai te dar detalhes ricos do erro
            throw new Exception($"Falha ao criar índice {IndexName}: {response.DebugInformation}");
        }
    }
}