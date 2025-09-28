using FluentAssertions;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Exceptions;
using Xunit;

namespace TechChallenge.Games.Testes.Core.Validators;

public class JogoValidatorTests
{
    [Fact]
    public void DeveValidarComSucesso_QuandoJogoValido()
    {
        // Arrange & Act - Usando o Builder que internamente valida
        var act = () => Jogo.New()
            .Nome("Super Mario Bros")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Xunit.Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void DeveFalhar_QuandoNomeVazio(string nomeInvalido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome(nomeInvalido)
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Xunit.Theory]
    [InlineData("AB")] // 2 caracteres
    [InlineData("A")]  // 1 caractere
    public void DeveFalhar_QuandoNomeMuitoPequeno(string nomePequeno)
    {
        // Act
        var act = () => Jogo.New()
            .Nome(nomePequeno)
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Fact]
    public void DevePassar_QuandoNomeTemTresCaracteres()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("ABC") // Exatamente 3 caracteres
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Xunit.Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void DeveFalhar_QuandoValorInvalido(decimal valorInvalido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(valorInvalido)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Xunit.Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(99.99)]
    [InlineData(1000)]
    public void DevePassar_QuandoValorPositivo(decimal valorValido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(valorValido)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Xunit.Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void DeveFalhar_QuandoDescontoMenorQueZero(int descontoInvalido)
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
    [InlineData(101)]
    [InlineData(150)]
    public void DeveFalhar_QuandoDescontoMaiorQueCem(int descontoInvalido)
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
    public void DevePassar_QuandoDescontoValidoEntreLimites(int descontoValido)
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(descontoValido)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DeveFalhar_QuandoDataCriacaoVazia()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.MinValue)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }

    [Fact]
    public void DevePassar_QuandoDataCriacaoValida()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DevePermitirDescricaoNula()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Descricao(null)
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DevePermitirDescricaoVazia()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("Jogo Teste")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Descricao("")
            .Build();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DeveTerMultiplosErros_QuandoMultiplosCamposInvalidos()
    {
        // Act - Tentando criar jogo com múltiplos campos inválidos
        var act = () => Jogo.New()
            .Nome("") // Nome inválido
            .Valor(-10) // Valor inválido
            .Desconto(-5) // Desconto inválido
            .DataCriacao(DateTime.MinValue) // Data inválida
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>();
    }
}
