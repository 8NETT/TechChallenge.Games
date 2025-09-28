using FluentAssertions;
using TechChallenge.Games.Core.Entity;
using Xunit;

namespace TechChallenge.Games.Testes.Core.Entity;

public class EntityBaseTests
{
    private class TestableEntity : EntityBase
    {
        public TestableEntity() { }

        public TestableEntity(int id, DateTime dataCriacao)
        {
            Id = id;
            DataCriacao = dataCriacao;
        }

        public void SetId(int id) => Id = id;
        public void SetDataCriacao(DateTime data) => DataCriacao = data;
    }

    [Fact]
    public void DeveInicializarComValoresPadrao()
    {
        // Act
        var entity = new TestableEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.DataCriacao.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void DevePermitirDefinirId()
    {
        // Arrange
        var entity = new TestableEntity();
        var expectedId = 123;

        // Act
        entity.SetId(expectedId);

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void DevePermitirDefinirDataCriacao()
    {
        // Arrange
        var entity = new TestableEntity();
        var expectedDate = DateTime.Now;

        // Act
        entity.SetDataCriacao(expectedDate);

        // Assert
        entity.DataCriacao.Should().BeCloseTo(expectedDate, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void DeveInicializarComValoresViaConstructor()
    {
        // Arrange
        var expectedId = 456;
        var expectedDate = DateTime.Now.AddDays(-1);

        // Act
        var entity = new TestableEntity(expectedId, expectedDate);

        // Assert
        entity.Id.Should().Be(expectedId);
        entity.DataCriacao.Should().Be(expectedDate);
    }

    [Xunit.Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void DevePermitirQualquerValorDeId(int id)
    {
        // Arrange
        var entity = new TestableEntity();

        // Act
        entity.SetId(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void DevePermitirDataCriacaoFutura()
    {
        // Arrange
        var entity = new TestableEntity();
        var dataFutura = DateTime.Now.AddDays(30);

        // Act
        entity.SetDataCriacao(dataFutura);

        // Assert
        entity.DataCriacao.Should().Be(dataFutura);
    }

    [Fact]
    public void DevePermitirDataCriacaoPassada()
    {
        // Arrange
        var entity = new TestableEntity();
        var dataPassada = DateTime.Now.AddYears(-10);

        // Act
        entity.SetDataCriacao(dataPassada);

        // Assert
        entity.DataCriacao.Should().Be(dataPassada);
    }

    [Fact]
    public void DevePermitirDataCriacaoMinima()
    {
        // Arrange
        var entity = new TestableEntity();
        var dataMinima = DateTime.MinValue;

        // Act
        entity.SetDataCriacao(dataMinima);

        // Assert
        entity.DataCriacao.Should().Be(dataMinima);
    }

    [Fact]
    public void DevePermitirDataCriacaoMaxima()
    {
        // Arrange
        var entity = new TestableEntity();
        var dataMaxima = DateTime.MaxValue;

        // Act
        entity.SetDataCriacao(dataMaxima);

        // Assert
        entity.DataCriacao.Should().Be(dataMaxima);
    }

    [Fact]
    public void DeveManterEstadoConsistente_AposMultiplasAlteracoes()
    {
        // Arrange
        var entity = new TestableEntity();
        var finalId = 999;
        var finalDate = DateTime.Now;

        // Act
        entity.SetId(1);
        entity.SetDataCriacao(DateTime.Now.AddDays(-1));
        entity.SetId(finalId);
        entity.SetDataCriacao(finalDate);

        // Assert
        entity.Id.Should().Be(finalId);
        entity.DataCriacao.Should().BeCloseTo(finalDate, TimeSpan.FromMilliseconds(1));
    }
}
