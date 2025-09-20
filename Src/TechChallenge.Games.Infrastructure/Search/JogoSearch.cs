using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Core.Documents;
using TechChallenge.Games.Core.Repository;
using TechChallenge.Games.Infrastructure.Configurations;
using TechChallenge.Games.Infrastructure.Repository;

namespace TechChallenge.Games.Infrastructure.Search;

public class JogoSearch(
    ElasticsearchClient es,
    ILogger<JogoSearch> logger,
    IUnitOfWork unitOfWork) : IJogoSearch
{
    public async Task<int> ReindexAllAsync(CancellationToken ct)
    {
        // 1) Lê tudo do SQL (pode paginar se o volume crescer)
        var jogos = await unitOfWork.JogoRepository.Query()
            .AsNoTracking()
            .ToListAsync(ct);

        // 2) Mapeia para documentos
        var docs = jogos.Select(j => new JogoDocument
        {
            Id = j.Id,
            Nome = j.Nome,
            Valor = Convert.ToDouble(j.Valor),
            Desconto = j.Desconto,
            DataCriacao = j.DataCriacao,
            Descricao = j.Descricao,
            Popularidade = 0,
            Keywords = null
        }).ToList();

        if (docs.Count == 0) return 0;

        // 3) Monta operações Bulk (Index) com ids estáveis
        var ops = new List<IBulkOperation>(capacity: docs.Count);
        foreach (var d in docs)
        {
            ops.Add(new BulkIndexOperation<JogoDocument>(d)
            {
                Id = d.Id.ToString()
            });
        }

        var bulkReq = new BulkRequest(ElasticSetup.IndexName)
        {
            Operations = ops
        };

        // 4) Executa o bulk
        var bulkRes = await es.BulkAsync(bulkReq, ct);

        if (!bulkRes.IsValidResponse || bulkRes.Errors)
        {
            // Loga itens com erro (se houver)
            var errors = bulkRes.Items.Where(i => i.Error is not null).Take(5).ToList();
            logger.LogWarning("Bulk com erros. Total={total}, ExemploErros={@errors}", bulkRes.Items.Count, errors);
        }

        return docs.Count;
    }
}