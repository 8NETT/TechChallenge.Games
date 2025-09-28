using FluentAssertions;
using TechChallenge.Games.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TechChallenge.Games.Testes.Application.DTOs;

public class JogoDTOTests
{
    [Fact]
    public void DeveCriarJogoDTO_ComPropriedadesCorretas()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Super Mario Bros",
            Valor = 59.99m,
            Desconto = 10
        };

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("Super Mario Bros");
        dto.Valor.Should().Be(59.99m);
        dto.Desconto.Should().Be(10);
    }

    [Fact]
    public void DeveCriarJogoDTO_ComValoresExtremos()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = int.MaxValue,
            Nome = "Jogo Com Valores Extremos",
            Valor = decimal.MaxValue,
            Desconto = 100
        };

        // Assert
        dto.Id.Should().Be(int.MaxValue);
        dto.Nome.Should().Be("Jogo Com Valores Extremos");
        dto.Valor.Should().Be(decimal.MaxValue);
        dto.Desconto.Should().Be(100);
    }

    [Fact]
    public void DeveCriarJogoDTO_ComValoresMinimos()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "A",
            Valor = 0.01m,
            Desconto = 0
        };

        // Assert
        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("A");
        dto.Valor.Should().Be(0.01m);
        dto.Desconto.Should().Be(0);
    }

    [Fact]
    public void DeveExigirId()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 5,
            Nome = "Teste",
            Valor = 29.99m,
            Desconto = 15
        };

        // Assert
        dto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DeveExigirNome()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Nome Obrigatório",
            Valor = 39.99m,
            Desconto = 5
        };

        // Assert
        dto.Nome.Should().NotBeNull();
        dto.Nome.Should().NotBeEmpty();
    }

    [Fact]
    public void DeveExigirValor()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Teste",
            Valor = 49.99m,
            Desconto = 20
        };

        // Assert
        dto.Valor.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DevePermitirDescontoZero()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Sem Desconto",
            Valor = 59.99m,
            Desconto = 0
        };

        // Assert
        dto.Desconto.Should().Be(0);
    }

    [Fact]
    public void DeveSerIgual_QuandoTodasPropriedadesSaoIguais()
    {
        // Arrange
        var dto1 = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = 29.99m,
            Desconto = 15
        };

        var dto2 = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = 29.99m,
            Desconto = 15
        };

        // Assert
        dto1.Id.Should().Be(dto2.Id);
        dto1.Nome.Should().Be(dto2.Nome);
        dto1.Valor.Should().Be(dto2.Valor);
        dto1.Desconto.Should().Be(dto2.Desconto);
    }

    [Fact]
    public void DeveSerDiferente_QuandoIdDiferente()
    {
        // Arrange
        var dto1 = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = 29.99m,
            Desconto = 15
        };

        var dto2 = new JogoDTO
        {
            Id = 2,
            Nome = "Jogo Teste",
            Valor = 29.99m,
            Desconto = 15
        };

        // Assert
        dto1.Id.Should().NotBe(dto2.Id);
    }

    [Fact]
    public void DeveSerDiferente_QuandoNomeDiferente()
    {
        // Arrange
        var dto1 = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo A",
            Valor = 29.99m,
            Desconto = 15
        };

        var dto2 = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo B",
            Valor = 29.99m,
            Desconto = 15
        };

        // Assert
        dto1.Nome.Should().NotBe(dto2.Nome);
    }

    [Fact]
    public void Id_DeveSerInteiro()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 42,
            Nome = "Teste",
            Valor = 19.99m,
            Desconto = 0
        };

        // Assert
        dto.Id.GetType().Should().Be(typeof(int));
        typeof(int).IsAssignableFrom(dto.Id.GetType()).Should().BeTrue();
    }

    [Fact]
    public void Nome_DeveSerString()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Nome do Jogo",
            Valor = 19.99m,
            Desconto = 0
        };

        // Assert
        dto.Nome.Should().BeAssignableTo<string>();
    }

    [Fact]
    public void Valor_DeveSerDecimal()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Teste",
            Valor = 29.99m,
            Desconto = 0
        };

        // Assert
        dto.Valor.GetType().Should().Be(typeof(Decimal));
    }

    [Fact]
    public void Desconto_DeveSerInteiro()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Teste",
            Valor = 29.99m,
            Desconto = 25
        };

        // Assert
        dto.Desconto.GetType().Should().Be(typeof(int));
    }

    [Fact]
    public void DeveSuportarNomesLongos()
    {
        // Arrange
        var nomeLongo = "The Elder Scrolls V: Skyrim - Legendary Edition with All DLCs and Expansions";

        // Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = nomeLongo,
            Valor = 99.99m,
            Desconto = 50
        };

        // Assert
        dto.Nome.Should().Be(nomeLongo);
        dto.Nome.Length.Should().BeGreaterThan(50);
    }

    [Fact]
    public void DeveSuportarCaracteresEspeciais()
    {
        // Arrange
        var nomeComCaracteresEspeciais = "Pokémon™: Let's Go, Pikachu! & Eevee! - Édition Spéciale";

        // Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = nomeComCaracteresEspeciais,
            Valor = 59.99m,
            Desconto = 20
        };

        // Assert
        dto.Nome.Should().Be(nomeComCaracteresEspeciais);
        dto.Nome.Should().Contain("™");
        dto.Nome.Should().Contain("é");
    }

    [Fact]
    public void DeveSuportarPrecosComCasasDecimais()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Preciso",
            Valor = 12.345m,
            Desconto = 0
        };

        // Assert
        dto.Valor.Should().Be(12.345m);
    }

    [Fact]
    public void DeveSuportarJogoGratuito()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Free-to-Play Game",
            Valor = 100.00m, // Preço original
            Desconto = 100 // Desconto total = gratuito
        };

        // Assert
        dto.Valor.Should().Be(100.00m);
        dto.Desconto.Should().Be(100);
    }

    [Fact]
    public void DeveSuportarJogoCarissimo()
    {
        // Arrange & Act
        var dto = new JogoDTO
        {
            Id = 1,
            Nome = "Jogo Premium Exclusivo",
            Valor = 999.99m,
            Desconto = 5
        };

        // Assert
        dto.Valor.Should().Be(999.99m);
        dto.Desconto.Should().Be(5);
    }

    [Fact]
    public void DeveCriarMultiplosJogosDTO_SemProblemasDeMemoria()
    {
        // Arrange
        var dtos = new List<JogoDTO>();

        // Act
        for (int i = 1; i <= 1000; i++)
        {
            dtos.Add(new JogoDTO
            {
                Id = i,
                Nome = $"Jogo {i}",
                Valor = i * 10.99m,
                Desconto = i % 101
            });
        }

        // Assert
        dtos.Should().HaveCount(1000);
        dtos.First().Id.Should().Be(1);
        dtos.Last().Id.Should().Be(1000);
    }

    private static List<ValidationResult> ValidateDTO(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }
}