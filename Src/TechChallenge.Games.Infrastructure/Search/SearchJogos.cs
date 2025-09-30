using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Core.Documents;
using TechChallenge.Games.Infrastructure.Configurations;

namespace TechChallenge.Games.Infrastructure.Search;

public class SearchJogos : ISearchJogos
{
    private readonly ElasticsearchClient _es;
    public SearchJogos(ElasticsearchClient es)
    {
        _es = es;
    }
    public async Task<SearchResult<JogoDocument>> SearchJogosAsync(
        string query,
        int from = 0,
        int size = 20)
    {
        var res = await _es.SearchAsync<JogoDocument>(s => s
            .Indices(ElasticSetup.IndexName)
            .From(from)
            .Size(size)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(f => f.Nome)
                    .Fields(f => f.Descricao)
                    .Query(query)
                    .Fuzziness("AUTO")
                    .Operator(Operator.Or)
                )
            )
        );

        return new SearchResult<JogoDocument>
        {
            Total = res.Total,
            Hits = res.Hits.Select(h => h.Source!).ToList()
        };
    }
}