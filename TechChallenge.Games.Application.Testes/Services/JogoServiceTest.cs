using Ardalis.Result;
using Moq;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Query.Domain.Documents;

namespace TechChallenge.Games.Application.Testes.Services
{
    public class JogoServiceTest : IClassFixture<JogoServiceFixture>
    {
        private readonly JogoServiceFixture _fixture;

        public JogoServiceTest(JogoServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ObterTodosAsync_Sucesso()
        {
            // Arrange
            var jogo = new JogoDocument
            {
                Id = Guid.NewGuid(),
                Nome = "Teste",
                DataLancamento = DateTime.Today,
                Preco = 100M,
                Desconto = 0,
                Valor = 100M
            };
            _fixture.QueryRepository.Setup(r => r.ObterTodosAsync(0, 10))
                .ReturnsAsync([jogo]);

            // Act
            var result = await _fixture.Service.ObterTodosAsync(0, 10);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(jogo.Id, result.First().Id);
        }

        [Fact]
        public async Task BuscarAsync_Sucesso()
        {
            // Arrange
            var dto = new BuscarJogoDTO
            {
                Termo = "Teste",
                Inicio = 0,
                Tamanho = 10
            };
            _fixture.QueryRepository.Setup(r => r.BuscarAsync(dto.Termo, dto.Inicio, dto.Tamanho))
                .ReturnsAsync(new List<JogoDocument>
                {
                    new JogoDocument
                    {
                        Id = Guid.NewGuid(),
                        Nome = "Teste",
                        DataLancamento = DateTime.Today,
                        Preco = 100M,
                        Desconto = 0,
                        Valor = 100M
                    }
                });

            // Act
            var result = await _fixture.Service.BuscarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async Task BuscarAsync_TermoVazio()
        {
            // Arrange
            var dto = new BuscarJogoDTO
            {
                Inicio = 0,
                Tamanho = 10
            };

            // Act
            var result = await _fixture.Service.BuscarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task BuscarAsync_InicioNegativo()
        {
            // Arrange
            var dto = new BuscarJogoDTO
            {
                Termo = "Teste",
                Inicio = -1,
                Tamanho = 10
            };

            // Act
            var result = await _fixture.Service.BuscarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task BuscarAsync_TamanhoInvalido()
        {
            // Arrange
            var dto = new BuscarJogoDTO
            {
                Termo = "Teste",
                Inicio = 0,
                Tamanho = 0
            };

            // Act
            var result = await _fixture.Service.BuscarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CadastrarAsync_Sucesso()
        {
            // Arrange
            var dto = new CadastrarJogoDTO
            {
                Nome = "Teste",
                DataLancamento = DateTime.Today,
                Preco = 100M
            };
            _fixture.QueryRepository.Setup(r => r.ObterPorNomeAsync(dto.Nome))
                .ReturnsAsync(value: null);
            _fixture.CommandRepository.Setup(r => r.SalvarAsync(It.IsAny<Jogo>()));
            _fixture.Producer.Setup(p => p.ProduceAsync(It.IsAny<JogoDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _fixture.Service.CadastrarAsync(dto);
            var jogo = result.Value;

            // Assert
            _fixture.Producer.Verify(p => p.ProduceAsync(It.IsAny<JogoDTO>()), Times.Once);
            _fixture.CommandRepository.Verify(r => r.SalvarAsync(It.IsAny<Jogo>()), Times.Once);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(dto.Nome, jogo.Nome);
        }

        [Fact]
        public async Task CadastrarAsync_NomeVazio()
        {
            // Arrange
            var dto = new CadastrarJogoDTO
            {
                Nome = string.Empty,
                DataLancamento = DateTime.Today,
                Preco = 100M
            };

            // Act
            var result = await _fixture.Service.CadastrarAsync(dto);

            // Assert
            _fixture.Producer.Verify(p => p.ProduceAsync(It.IsAny<JogoDTO>()), Times.Never);
            _fixture.CommandRepository.Verify(r => r.SalvarAsync(It.IsAny<Jogo>()), Times.Never);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task CadastrarAsync_PrecoNegativo()
        {
            // Arrange
            var dto = new CadastrarJogoDTO
            {
                Nome = "Teste",
                DataLancamento = DateTime.Today,
                Preco = -100M
            };

            // Act
            var result = await _fixture.Service.CadastrarAsync(dto);

            // Assert
            _fixture.Producer.Verify(p => p.ProduceAsync(It.IsAny<JogoDTO>()), Times.Never);
            _fixture.CommandRepository.Verify(r => r.SalvarAsync(It.IsAny<Jogo>()), Times.Never);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task AlterarDadosAsync_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();
            var dto = new AlterarDadosDTO
            {
                Id = jogo.Id,
                Nome = "Teste Alterado",
                DataLancamento = DateTime.Today.AddDays(-1)
            };
            _fixture.CommandRepository.Setup(r => r.ObterPorIdAsync(jogo.Id))
                .ReturnsAsync(jogo);
            _fixture.QueryRepository.Setup(r => r.ObterPorNomeAsync(dto.Nome))
                .ReturnsAsync(value: null);
            _fixture.CommandRepository.Setup(r => r.SalvarAsync(jogo));
            _fixture.Producer.Setup(p => p.ProduceAsync(It.IsAny<JogoDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _fixture.Service.AlterarDadosAsync(dto);
            var jogoAlterado = result.Value;

            // Assert
            _fixture.CommandRepository.Verify(r => r.SalvarAsync(jogo), Times.Once);
            _fixture.Producer.Verify(p => p.ProduceAsync(It.IsAny<JogoDTO>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(dto.Nome, jogoAlterado.Nome);
            Assert.Equal(dto.DataLancamento, jogoAlterado.DataLancamento);
        }

        [Fact]
        public async Task AlterarDadosAsync_NaoLocalizado()
        {
            // Arrange
            var dto = new AlterarDadosDTO
            {
                Id = Guid.NewGuid(),
                Nome = "Teste Alterado",
                DataLancamento = DateTime.Today.AddDays(-1)
            };
            _fixture.CommandRepository.Setup(r => r.ObterPorIdAsync(dto.Id))
                .ReturnsAsync(value: null);

            // Act
            var result = await _fixture.Service.AlterarDadosAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsNotFound());
        }

        [Fact]
        public async Task AlterarDadosAsync_NomeExistente()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();
            var jogoExistente = new JogoDocument
            {
                Nome = "Teste Alterado",
                DataLancamento = DateTime.Today,
                Preco = 100M,
                Desconto = 0,
                Valor = 100M
            };
            var dto = new AlterarDadosDTO
            {
                Id = jogo.Id,
                Nome = "Teste Alterado",
                DataLancamento = DateTime.Today.AddDays(-1)
            };
            _fixture.CommandRepository.Setup(r => r.ObterPorIdAsync(jogo.Id))
                .ReturnsAsync(jogo);
            _fixture.QueryRepository.Setup(r => r.ObterPorNomeAsync(dto.Nome))
                .ReturnsAsync(jogoExistente);

            // Act
            var result = await _fixture.Service.AlterarDadosAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsConflict());
        }
    }
}
