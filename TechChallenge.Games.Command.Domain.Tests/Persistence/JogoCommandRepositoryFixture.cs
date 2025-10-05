using Moq;
using TechChallenge.Games.Command.Domain.Persistence;

namespace TechChallenge.Games.Command.Domain.Tests.Persistence
{
    public class JogoCommandRepositoryFixture
    {
        public Mock<IEventStore> EventStore { get; } = new();
        public JogoCommandRepository Repository { get; }

        public JogoCommandRepositoryFixture()
        {
            Repository = new JogoCommandRepository(EventStore.Object);
        }
    }
}
