namespace TechChallenge.Games.Application.Contracts;

public interface IJogoSearch
{
    Task<int> ReindexAllAsync(CancellationToken ct);
}