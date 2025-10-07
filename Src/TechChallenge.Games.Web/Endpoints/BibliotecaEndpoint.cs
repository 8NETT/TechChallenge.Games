using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechChallenge.Games.Application.Contracts;

namespace TechChallenge.Games.Web.Endpoints
{
    public static class BibliotecaEndpoint
    {
        public static RouteGroupBuilder MapBibliotecaEndpoints(this IEndpointRouteBuilder app)
        {
            var bibliotecas = app.MapGroup("/biblioteca")
                .RequireAuthorization(); // substitui [Authorize] no controller

            // GET: /biblioteca/{usuarioId}
            bibliotecas.MapGet("/{usuarioId:Guid}", async ([FromRoute] int usuarioId, IBibliotecaService service) =>
            {
                try
                {
                    var result = await service.ObterBibliotecaAsync(usuarioId);

                    if (result.IsNotFound())
                        return Results.NotFound(result.Errors);

                    return Results.Ok(result.Value);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(new { error = e.Message });
                }
            })
            .WithOpenApi(op => new(op)
            {
                OperationId = "GetBibliotecaPorUsuarioIdAsync",
                Summary = "Biblioteca encontrada com sucesso",
                Description = "Busca uma biblioteca específica pelo ID do usuário"
            })
            .Produces((int)HttpStatusCode.OK)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.BadRequest);

            return bibliotecas;
        }
    }
}
