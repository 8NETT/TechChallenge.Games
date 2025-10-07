using System.Net;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Endpoints;

public static class JogoEndpoints
{
    public static RouteGroupBuilder MapJogoEndpoints(this IEndpointRouteBuilder app)
    {
        var jogos = app.MapGroup("/jogo")
            .RequireAuthorization(); // substitui [Authorize] no controller

        // GET: /jogo
        jogos.MapGet("/", async (IJogoService jogoService, [FromQuery] int inicio = 0, [FromQuery] int tamanho = 10) =>
        {
            try
            {

                return Results.Ok(await jogoService.ObterTodosAsync(new ObterTodosDto { Inicio = inicio, Tamanho = tamanho}));
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .WithOpenApi(op => new(op)
        {
            OperationId = "GetJogoAsync",
            Summary = "Jogos listados com sucesso",
            Description = "Retorna todos os jogos cadastrados"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.BadRequest);

        // GET: /jogo/{id}
        jogos.MapGet("/{id:Guid}", async ([FromRoute] Guid id, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.ObterPorIdAsync(id);

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
            OperationId = "GetJogoPorIdAsync",
            Summary = "Jogo encontrado com sucesso",
            Description = "Busca um jogo específico pelo ID"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.NotFound)
        .Produces((int)HttpStatusCode.BadRequest);

        // POST: /jogo
        jogos.MapPost("/", async (CadastrarJogoDTO dto, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.CadastrarAsync(dto);

                if (result.IsInvalid())
                    return Results.BadRequest(result.Errors);
                if (result.IsConflict())
                    return Results.Conflict(result.Errors);

                return Results.Ok(result.Value);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .RequireAuthorization("Administrador")
        .WithOpenApi(op => new(op)
        {
            OperationId = "PostJogoAsync",
            Summary = "Jogo cadastrado com sucesso",
            Description = "Cadastra um novo jogo no sistema"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.Conflict)
        .Produces((int)HttpStatusCode.BadRequest);

        // PUT: /jogo
        jogos.MapPut("/", async (AlterarDadosDTO dto, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.AlterarDadosAsync(dto);

                if (result.IsInvalid())
                    return Results.BadRequest(result.Errors);
                if (result.IsNotFound())
                    return Results.NotFound(result.Errors);
                if (result.IsConflict())
                    return Results.Conflict(result.Errors);

                return Results.Ok(result.Value);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .RequireAuthorization("Administrador")
        .WithOpenApi(op => new(op)
        {
            OperationId = "PutJogoAsync",
            Summary = "Jogo atualizado com sucesso",
            Description = "Atualiza os dados de um jogo existente"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.NotFound)
        .Produces((int)HttpStatusCode.Conflict)
        .Produces((int)HttpStatusCode.BadRequest);

        // PUT: /jogo/preco
        jogos.MapPut("/preco", async (AlterarPrecoDTO dto, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.AlterarPrecoAsync(dto);
                if (result.IsInvalid())
                    return Results.BadRequest(result.Errors);
                if (result.IsNotFound())
                    return Results.NotFound(result.Errors);
                return Results.Ok(result.Value);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .RequireAuthorization("Administrador")
        .WithOpenApi(op => new(op)
        {
            OperationId = "PutPrecoAsync",
            Summary = "Preço atualizado com sucesso",
            Description = "Atualiza o preço de um jogo existente"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.NotFound)
        .Produces((int)HttpStatusCode.Conflict)
        .Produces((int)HttpStatusCode.BadRequest);

        // PUT: /jogo/desconto
        jogos.MapPut("/desconto", async (AplicarDescontoDTO dto, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.AplicarDescontoAsync(dto);
                if (result.IsInvalid())
                    return Results.BadRequest(result.Errors);
                if (result.IsNotFound())
                    return Results.NotFound(result.Errors);
                return Results.Ok(result.Value);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .RequireAuthorization("Administrador")
        .WithOpenApi(op => new(op)
        {
            OperationId = "PutDescontoAsync",
            Summary = "Desconto aplicado com sucesso",
            Description = "Atualiza o desconto de um jogo existente"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.NotFound)
        .Produces((int)HttpStatusCode.Conflict)
        .Produces((int)HttpStatusCode.BadRequest);

        // DELETE: /jogo/{id}
        jogos.MapDelete("/{id:Guid}", async (Guid id, IJogoService jogoService) =>
        {
            try
            {
                var result = await jogoService.DeletarAsync(id);

                if (result.IsNotFound())
                    return Results.NotFound(result.Errors);

                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { error = e.Message });
            }
        })
        .RequireAuthorization("Administrador")
        .WithOpenApi(op => new(op)
        {
            OperationId = "DeleteJogoAsync",
            Summary = "Jogo deletado com sucesso",
            Description = "Remove um jogo do sistema pelo ID"
        })
        .Produces((int)HttpStatusCode.OK)
        .Produces((int)HttpStatusCode.NotFound)
        .Produces((int)HttpStatusCode.BadRequest);

        return jogos;
    }
}