using Elastic.Clients.Elasticsearch;
using TechChallenge.Games.Core.Documents;
using TechChallenge.Games.Infrastructure.Configurations;

namespace TechChallenge.Games.Infrastructure.Search;

public class SearchJogos
{
    public async Task<SearchResult<JogoDocument>> SearchJogosAsync(
        ElasticsearchClient es,
        string query,
        int from = 0,
        int size = 20)
    {
        var res = await es.SearchAsync<JogoDocument>(s => s
            .Indices(ElasticSetup.IndexName)
            .From(from)
            .Size(size)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(f => f.Nome)
                    .Fields(f => f.Descricao)
                    .Query(query)
                    .Fuzziness("AUTO")
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