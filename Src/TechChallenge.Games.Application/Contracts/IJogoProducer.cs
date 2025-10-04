using TechChallenge.Games.Application.DTOs;

namespace TechChallenge.Games.Application.Contracts
{
    public interface IJogoProducer
    {
        Task ProduceAsync(JogoDTO dto);
    }
}
