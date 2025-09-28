using FluentAssertions;
using Ardalis.Result;
using TechChallenge.Games.Application.Services;
using TechChallenge.Games.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TechChallenge.Games.Testes.Application.Services;

public class BaseServiceTests
{
    private class TestableBaseService : BaseService
    {
        public new bool TryValidate<T>(T dto, out Result result)
        {
            return base.TryValidate(dto, out result);
        }
    }

    private readonly TestableBaseService _baseService;

    public BaseServiceTests()
    {
        _baseService = new TestableBaseService();
    }
    
    [Fact]
    public void TryValidate_DeveRetornarTrue_QuandoCadastrarJogoDTOValido()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Super Mario Bros",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Jogo clássico da Nintendo"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TryValidate_DeveRetornarTrue_QuandoAlterarJogoDTOValido()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "The Legend of Zelda",
            Valor = 69.99m,
            Desconto = 15,
            Descricao = "Jogo de aventura épico"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TryValidate_DeveRetornarTrue_QuandoDTOComDescricaoNula()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "God of War",
            Valor = 49.99m,
            Desconto = 20,
            Descricao = null
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryValidate_DeveRetornarTrue_QuandoDTOComDescontoZero()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Sem Desconto",
            Valor = 99.99m,
            Desconto = 0,
            Descricao = "Jogo com preço cheio"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoNomeVazio()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().HaveCountGreaterThan(0);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve ser preenchido"));
    }

    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoNomeMuitoCurto()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "AB", // Menos de 3 caracteres
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve ter entre 3 a 100"));
    }

    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoValorNegativo()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = -10.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve ser positivo"));
    }

    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoDescontoNegativo()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = -5,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve estar entre 0 e 100"));
    }

    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoDescontoMaiorQue100()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 150,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve estar entre 0 e 100"));
    }

    [Fact]
    public void TryValidate_DeveRetornarFalse_QuandoDescricaoMuitoLonga()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = new string('A', 501) // Mais de 500 caracteres
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().Contain(e => e.ErrorMessage.Contains("deve ter no máximo 500 caracteres"));
    }

    [Fact]
    public void TryValidate_DeveRetornarTodosOsErros_QuandoMultiplosCamposInvalidos()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "", // Nome inválido
            Valor = -10m, // Valor inválido
            Desconto = 150, // Desconto inválido
            Descricao = new string('A', 501) // Descrição muito longa
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().HaveCount(4); // Todos os campos inválidos
    }
    
    [Fact]
    public void TryValidate_DeveLancarException_QuandoDTONulo()
    {
        // Act
        var act = () => _baseService.TryValidate<CadastrarJogoDTO>(null!, out var result);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryValidate_DevePermitirNomeComExatamente3Caracteres()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "ABC",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryValidate_DevePermitirNomeComExatamente100Caracteres()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = new string('A', 100),
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryValidate_DevePermitirDescricaoComExatamente500Caracteres()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = new string('A', 500)
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryValidate_DevePermitirDescontoExatamente100()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Gratuito",
            Valor = 59.99m,
            Desconto = 100,
            Descricao = "Jogo com desconto máximo"
        };

        // Act
        var isValid = _baseService.TryValidate(dto, out var result);

        // Assert
        isValid.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }
}
