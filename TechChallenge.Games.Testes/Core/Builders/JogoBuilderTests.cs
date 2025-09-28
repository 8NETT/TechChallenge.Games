using FluentAssertions;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Builders;
using TechChallenge.Games.Core.Exceptions;
using Xunit;

namespace TechChallenge.Games.Testes.Core.Builders;

public class JogoBuilderTests
{
    [Fact]
    public void DeveCriarJogo_QuandoDadosValidos()
    {
        // Arrange
        var nome = "Super Mario Bros";
        var valor = 59.99m;
        var desconto = 10;
        var descricao = "Jogo clássico da Nintendo";
        var dataCriacao = DateTime.Now;

        // Act
        var jogo = Jogo.New()
            .Nome(nome)
            .Valor(valor)
            .Desconto(desconto)
            .Descricao(descricao)
            .DataCriacao(dataCriacao)
            .Build();

        // Assert
        jogo.Should().NotBeNull();
        jogo.Nome.Should().Be(nome);
        jogo.Valor.Should().Be(valor);
        jogo.Desconto.Should().Be(desconto);
        jogo.Descricao.Should().Be(descricao);
        jogo.DataCriacao.Should().BeCloseTo(dataCriacao, TimeSpan.FromSeconds(1));
    }
    
    [Xunit.Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("AB")] // Menos de 3 caracteres
    public void DeveLancarExcecao_QuandoNomeInvalido(string nomeInvalido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome(nomeInvalido)
            .Valor(59.99m)
            .Desconto(0)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Xunit.Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-0.01)]
    public void DeveLancarExcecao_QuandoValorInvalido(decimal valorInvalido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(valorInvalido)
            .Desconto(0)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Xunit.Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void DeveLancarExcecao_QuandoDescontoInvalido(int descontoInvalido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(descontoInvalido)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Xunit.Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void DeveCriarJogo_QuandoDescontoValido(int descontoValido)
    {
        // Act
        var jogo = Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(descontoValido)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        jogo.Should().NotBeNull();
        jogo.Desconto.Should().Be(descontoValido);
    }

    [Fact]
    public void DeveLancarExcecao_QuandoDataCriacaoVazia()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(0)
            .DataCriacao(DateTime.MinValue)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Fact]
    public void DeveValidarCorretamente_AntesDeBuild()
    {
        // Arrange
        var builder = Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now);

        // Act
        var validationResult = builder.Validate();

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeveRetornarErrosValidacao_QuandoDadosInvalidos()
    {
        // Arrange
        var builder = Jogo.New()
            .Nome("") // Nome inválido
            .Valor(-10) // Valor inválido
            .Desconto(-5); // Desconto inválido

        // Act
        var validationResult = builder.Validate();

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void DevePermitirDescricaoNula()
    {
        // Act
        var jogo = Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(0)
            .DataCriacao(DateTime.Now)
            .Descricao(null)
            .Build();

        // Assert
        jogo.Should().NotBeNull();
        jogo.Descricao.Should().BeNull();
    }

    [Fact]
    public void DevePermitirDescricaoVazia()
    {
        // Act
        var jogo = Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(0)
            .DataCriacao(DateTime.Now)
            .Descricao("")
            .Build();

        // Assert
        jogo.Should().NotBeNull();
        jogo.Descricao.Should().Be("");
    }
}
