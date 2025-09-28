using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using TechChallenge.Games.Application.DTOs;
using Xunit;

namespace TechChallenge.Games.Testes.Application.DTOs;

public class CadastrarJogoDTOTests
{
    [Fact]
    public void DeveSerValido_QuandoTodosCamposCorretos()
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
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Xunit.Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void DeveSerInvalido_QuandoNomeVazioOuNulo(string nomeInvalido)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = nomeInvalido!,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Nome)) &&
            vr.ErrorMessage!.Contains("deve ser preenchido"));
    }

    [Xunit.Theory]
    [InlineData("AB")] // 2 caracteres
    [InlineData("A")] // 1 caractere
    public void DeveSerInvalido_QuandoNomeMuitoPequeno(string nomePequeno)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = nomePequeno,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Nome)) &&
            vr.ErrorMessage!.Contains("entre 3 a 100 caracteres"));
    }

    [Fact]
    public void DeveSerInvalido_QuandoNomeMuitoGrande()
    {
        // Arrange
        var nomeGrande = new string('A', 101); // 101 caracteres
        var dto = new CadastrarJogoDTO
        {
            Nome = nomeGrande,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Nome)) &&
            vr.ErrorMessage!.Contains("entre 3 a 100 caracteres"));
    }

    [Xunit.Theory]
    [InlineData(3)] // Mínimo
    [InlineData(50)] // Meio termo
    [InlineData(100)] // Máximo
    public void DeveSerValido_QuandoNomeTemTamanhoCorreto(int tamanho)
    {
        // Arrange
        var nome = new string('A', tamanho);
        var dto = new CadastrarJogoDTO
        {
            Nome = nome,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Nome)))
            .Should().BeEmpty();
    }

    [Xunit.Theory]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void DeveSerInvalido_QuandoValorNaoPositivo(decimal valorInvalido)
    {
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = valorInvalido,
            Desconto = 10,
            Descricao = "Descrição"
        };

        var validationResults = ValidateDTO(dto);

        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Valor)) &&
            vr.ErrorMessage!.Contains("deve ser positivo"));
    }


    [Xunit.Theory]
    [InlineData(0.01)]
    [InlineData(10.99)]
    [InlineData(199.99)]
    public void DeveSerValido_QuandoValorPositivo(decimal valorValido)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = valorValido,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Valor)))
            .Should().BeEmpty();
    }

    [Xunit.Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(101)]
    [InlineData(150)]
    public void DeveSerInvalido_QuandoDescontoForaDosLimites(int descontoInvalido)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = descontoInvalido,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Desconto)) &&
            vr.ErrorMessage!.Contains("entre 0 e 100"));
    }

    [Xunit.Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void DeveSerValido_QuandoDescontoDentroLimites(int descontoValido)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = descontoValido,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Desconto)))
            .Should().BeEmpty();
    }

    [Fact]
    public void DeveSerValido_QuandoDescricaoNula()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = null
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Descricao)))
            .Should().BeEmpty();
    }

    [Fact]
    public void DeveSerValido_QuandoDescricaoVazia()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = ""
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Descricao)))
            .Should().BeEmpty();
    }

    [Fact]
    public void DeveSerInvalido_QuandoDescricaoMuitoGrande()
    {
        // Arrange
        var descricaoGrande = new string('A', 501); // 501 caracteres
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = descricaoGrande
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().ContainSingle(vr =>
            vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Descricao)) &&
            vr.ErrorMessage!.Contains("no máximo 500 caracteres"));
    }

    [Xunit.Theory]
    [InlineData(1)]
    [InlineData(250)]
    [InlineData(500)] // Máximo permitido
    public void DeveSerValido_QuandoDescricaoTamanhoCorreto(int tamanho)
    {
        // Arrange
        var descricao = new string('A', tamanho);
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = descricao
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Descricao)))
            .Should().BeEmpty();
    }

    [Fact]
    public void DeveTerMultiplosErros_QuandoMultiplosCamposInvalidos()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "", // Inválido
            Valor = -10, // Inválido
            Desconto = 150, // Inválido
            Descricao = new string('A', 501) // Inválido
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().HaveCountGreaterOrEqualTo(4);
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Nome)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Valor)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Desconto)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(CadastrarJogoDTO.Descricao)));
    }

    private static List<ValidationResult> ValidateDTO(object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, context, results, true);
        return results;
    }
}