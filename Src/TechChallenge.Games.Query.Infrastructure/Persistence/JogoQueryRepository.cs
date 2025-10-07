using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using TechChallenge.Games.Query.Domain.Documents;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Query.Infrastructure.Persistence
{
    public sealed class JogoQueryRepository : IJogoQueryRepository
    {
        private ElasticsearchClient _client;
        private string _indexName;

        public JogoQueryRepository(ElasticsearchClient client, string indexName)
        {
            _client = client;
            _indexName = indexName;
        }

        public async Task<IEnumerable<JogoDocument>> ObterTodosAsync(int from, int size)
        {
            var response = await _client.SearchAsync<JogoDocument>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(size)
                .Query(q => q.MatchAll()));

            if (!response.IsValidResponse)
                return Enumerable.Empty<JogoDocument>();

            return response.Documents;
        }

        public async Task<JogoDocument?> ObterPorIdAsync(Guid id)
        {
            var response = await _client.GetAsync<JogoDocument>(id.ToString(), g => g.Index(_indexName));

            if (!response.IsValidResponse)
                return null;

            return response.Source;
        }

        public async Task<JogoDocument?> ObterPorNomeAsync(string nome)
        {
            var response = await _client.SearchAsync<JogoDocument>(s => s
                .Indices(_indexName)
                .Size(1)
                .Query(q => q
                    .Match(m => m
                        .Field(j => j.Nome)
                        .Query(nome)
                        .Operator(Operator.And)
                    )));

            if (!response.IsValidResponse || !response.Documents.Any())
                return null;

            return response.Documents.First();
        }

        public async Task<IEnumerable<JogoDocument>> BuscarAsync(string query, int from, int size)
        {
            var response = await _client.SearchAsync<JogoDocument>(s => s
                .Indices(_indexName)
                .From(from)
                .Size(size)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Fields(j => j.Nome, j => j.Descricao)
                        .Query(query)
                        .Fuzziness("AUTO")
                        .Operator(Operator.Or)
            )));


            if (!response.IsValidResponse)
                return Enumerable.Empty<JogoDocument>();

            return response.Documents;
        }

        public async Task<bool> UpsertAsync(JogoDocument jogo)
        {
            var response = await _client.IndexAsync(jogo, s => s
                .Index(_indexName)
                .Id(jogo.Id.ToString()));

            return response.IsValidResponse;
        }
    }
}
