using Moq;
using TechChallenge.Games.Application.Contracts;
using TechChallenge.Games.Application.Services;
using TechChallenge.Games.Command.Domain.Persistence;
using TechChallenge.Games.Query.Domain.Persistence;

namespace TechChallenge.Games.Application.Testes.Services
{
    public class JogoServiceFixture
    {
        public Mock<IJogoQueryRepository> QueryRepository { get; } = new();
        public Mock<IJogoProducer> Producer { get; } = new();
        public Mock<IJogoCommandRepository> CommandRepository { get; } = new();
        public JogoService Service { get; }

        public JogoServiceFixture()
        {
            Service = new JogoService(CommandRepository.Object, QueryRepository.Object, Producer.Object);
        }
    }
}
