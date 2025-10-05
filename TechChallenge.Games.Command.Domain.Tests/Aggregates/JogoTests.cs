using TechChallenge.Games.Command.Domain.Aggregates;

namespace TechChallenge.Games.Command.Domain.Tests.Aggregates
{
    public class JogoTests 
    {
        [Fact]
        public void AlterarDados_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            jogo.AlterarDados(
                nome: "Novo Teste",
                descricao: "Teste",
                dataLancamento: DateTime.Today.AddDays(1));

            // Assert
            Assert.Equal(2, jogo.GetUncommittedEvents().Count());
            Assert.Equal("Novo Teste", jogo.Nome);
            Assert.Equal("Teste", jogo.Descricao);
            Assert.Equal(DateTime.Today.AddDays(1), jogo.DataLancamento);
            Assert.Equal(100M, jogo.Preco);
        }

        [Fact]
        public void AlterarDados_CamposVazios()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            var ex = Record.Exception(() => 
                jogo.AlterarDados());

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void AlterarPreco_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            jogo.AlterarPreco(200M);

            // Assert
            Assert.Equal(2, jogo.GetUncommittedEvents().Count());
            Assert.Equal(200M, jogo.Preco);
        }

        [Fact]
        public void AlterarPreco_ValorNegativo()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            var ex = Record.Exception(() => jogo.AlterarPreco(-100M));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void AplicarDesconto_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            jogo.AplicarDesconto(10);

            // Assert
            Assert.Equal(2, jogo.GetUncommittedEvents().Count());
            Assert.Equal(100M, jogo.Preco);
            Assert.Equal(10, jogo.Desconto);
            Assert.Equal(90M, jogo.Valor);
        }

        [Fact]
        public void AplicarDesconto_ValorNegativo()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            var ex = Record.Exception(() => jogo.AplicarDesconto(-10));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void Remover_Sucesso()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            jogo.Remover();

            // Assert
            Assert.Equal(2, jogo.GetUncommittedEvents().Count());
            Assert.True(jogo.Deletado);
        }

        [Fact]
        public void Remover_Falha()
        {
            // Arrange
            var jogo = Jogo.Novo
                .Nome("Teste")
                .DataLancamento(DateTime.Today)
                .Preco(100M)
                .Build();

            // Act
            jogo.Remover();
            var ex = Record.Exception(jogo.Remover);

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
        }
    }
}
