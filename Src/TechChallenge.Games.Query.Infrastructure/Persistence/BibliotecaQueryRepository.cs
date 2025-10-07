using Elastic.Clients.Elasticsearch;
using TechChallenge.Games.Query.Domain.Documents;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Query.Infrastructure.Persistence
{
    public class BibliotecaQueryRepository : IBibliotecaQueryRepository
    {
        private ElasticsearchClient _client;
        private string _indexName;

        public BibliotecaQueryRepository(ElasticsearchClient client, string indexName)
        {
            _client = client;
            _indexName = indexName;
        }

        public async Task<BibliotecaDocument?> ObterPorUsuarioIdAsync(int usuarioId)
        {
            var response = await _client.SearchAsync<BibliotecaDocument>(s => s
                        .Indices(_indexName)
                        .Query(q => q
                            .Term(t => t
                                .Field(f => f.UsuarioId)
                                .Value(usuarioId)
                            )
                        )
                        .Size(1)
                    );

            if (!response.IsValidResponse || response.Documents.Count == 0)
                return null;

            return response.Documents.FirstOrDefault();

        }

        public async Task<bool> UpsertAsync(BibliotecaDocument biblioteca)
        {
            var response = await _client.IndexAsync(biblioteca, s => s
                .Index(_indexName)
                .Id(biblioteca.UsuarioId.ToString()));

            return response.IsValidResponse;
        }
    }
}
