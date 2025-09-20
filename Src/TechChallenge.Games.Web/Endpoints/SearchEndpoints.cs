using TechChallenge.Games.Application.Contracts;

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

        return app;
    }
}