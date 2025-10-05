using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Command.Domain.Exceptions;

namespace TechChallenge.Games.Command.Domain.Tests.Builders
{
    public class JogoBuilderTest
    {
        [Fact]
        public void Build_Sucesso()
        {
            // Act
            var ex = Record.Exception(() => Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Build_Failure_NomeVazio()
        {
            // Act
            var ex = Record.Exception(() => Jogo.Novo
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build());

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<EstadoInvalidoException>(ex);
        }

        [Fact]
        public void Build_Failure_PrecoNegativo()
        {
            // Act
            var ex = Record.Exception(() => Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(-100M)
                .Build());

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<EstadoInvalidoException>(ex);
        }
    }
}
