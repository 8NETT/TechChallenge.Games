using Elastic.Clients.Elasticsearch;
using TechChallenge.Games.Core.Documents;

namespace TechChallenge.Games.Application.Contracts
{
    public interface ISearchJogos
    {
       Task<SearchResult<JogoDocument>> SearchJogosAsync(
            string query,
            int from = 0,
            int size = 20);
    }
}
