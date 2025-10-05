using Moq;
using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Command.Domain.Events;

namespace TechChallenge.Games.Command.Domain.Tests.Persistence
{
    public class JogoCommandRepositoryTest : IClassFixture<JogoCommandRepositoryFixture>
    {
        private readonly JogoCommandRepositoryFixture _fixture;

        public JogoCommandRepositoryTest(JogoCommandRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ObterPorIdAsync_Sucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var events = new List<BaseEvent>
            {
                new JogoCriadoEvent
                {
                    AggregateId = id,
                    Nome = "Teste",
                    DataLancamento = DateTime.Today,
                    Preco = 100M
                }
            };

            _fixture.EventStore.Setup(e => e.GetEventsAsync(id)).ReturnsAsync(events);

            // Act
            var jogo = await _fixture.Repository.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(jogo);
            Assert.Equal(id, jogo.Id);
            Assert.Equal("Teste", jogo.Nome);
            Assert.Equal(DateTime.Today, jogo.DataLancamento);
            Assert.Equal(100M, jogo.Preco);
            Assert.Empty(jogo.GetUncommittedEvents());
        }

        [Fact]
        public async Task ObterPorIdAsync_NaoLocalizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            _fixture.EventStore.Setup(e => e.GetEventsAsync(id)).ReturnsAsync(Enumerable.Empty<BaseEvent>());

            // Act
            var jogo = await _fixture.Repository.ObterPorIdAsync(id);

            // Assert
            Assert.Null(jogo);
        }

        [Fact]
        public async Task SalvarJogoAsync_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();
            _fixture.EventStore.Setup(e => e.AppendAsync(It.IsAny<JogoCriadoEvent>()));

            // Act
            await _fixture.Repository.SalvarAsync(jogo);

            // Assert
            _fixture.EventStore.Verify(e => e.AppendAsync(It.IsAny<JogoCriadoEvent>()), Times.Once);
            Assert.Empty(jogo.GetUncommittedEvents());
        }
    }
}
