using Microsoft.AspNetCore.Mvc;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Core.Documents;

namespace TechChallenge.Games.Web.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/search/reindex", async (IJogoSearch indexer, CancellationToken ct) =>
            {
                var count = await indexer.ReindexAllAsync(ct);
                return Results.Ok(new { indexed = count });
            })
            .WithName("ReindexJogos")
            .WithSummary("Reindexa todos os jogos do SQL para o Elasticsearch");

        app.MapPost("/search", async (ISearchJogos jogos,[FromBody] SearchRequest request) =>
        {
            try
            {
                var result = await jogos.SearchJogosAsync(request.Termo,request.From,request.Size);
                return Results.Ok(result);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }

        })
            .WithOpenApi()
            .WithName("SearchJogos")
            .WithSummary("Busca jogos no Elasticsearch")
            .Produces<SearchResult<JogoDocument>>(StatusCodes.Status200OK);

        return app;
    }
}