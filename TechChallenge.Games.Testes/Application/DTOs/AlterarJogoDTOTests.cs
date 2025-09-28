using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using TechChallenge.Games.Application.DTOs;
using Xunit;

namespace TechChallenge.Games.Testes.Application.DTOs;

public class AlterarJogoDTOTests
{
    [Fact]
    public void DeveSerValido_QuandoTodosCamposCorretos()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
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
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void DeveSerValido_QuandoIdPositivo(int idValido)
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = idValido,
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Id)))
            .Should().BeEmpty();
    }

    [Fact]
    public void DeveHerdarValidacoesDeCadastrarJogoDTO()
    {
        // Arrange - Testando se as validações da classe base funcionam
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "", // Inválido - herdado de CadastrarJogoDTO
            Valor = -10, // Inválido - herdado de CadastrarJogoDTO
            Desconto = 150, // Inválido - herdado de CadastrarJogoDTO
            Descricao = new string('A', 501) // Inválido - herdado de CadastrarJogoDTO
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().HaveCountGreaterOrEqualTo(4);
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Nome)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Valor)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Desconto)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Descricao)));
    }
    
    [Fact]
    public void DeveSerValido_ComDescricaoNula()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = null
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void DeveSerValido_ComDescricaoVazia()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = ""
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void DeveSerValido_ComValoresLimite()
    {
        // Arrange - Testando valores nos limites permitidos
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "ABC", // Mínimo de 3 caracteres
            Valor = 0.01m, // Mínimo positivo
            Desconto = 100, // Máximo desconto
            Descricao = new string('A', 500) // Máximo descrição
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void DevePermitirDescontoZero()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Jogo Sem Desconto",
            Valor = 59.99m,
            Desconto = 0, // Desconto mínimo permitido
            Descricao = "Sem desconto"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Desconto)))
            .Should().BeEmpty();
    }

    [Xunit.Theory]
    [InlineData("Game")]
    [InlineData("Super Mario Bros Ultimate")]
    [InlineData("The Legend of Zelda: Breath of the Wild")]
    public void DeveSerValido_ComNomesVariados(string nome)
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = nome,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Nome)))
            .Should().BeEmpty();
    }

    [Xunit.Theory]
    [InlineData(1.50)]
    [InlineData(19.99)]
    [InlineData(99.99)]
    [InlineData(199.99)]
    public void DeveSerValido_ComValoresVariados(decimal valor)
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Jogo Teste",
            Valor = valor,
            Desconto = 10,
            Descricao = "Descrição"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Where(vr => vr.MemberNames.Contains(nameof(AlterarJogoDTO.Valor)))
            .Should().BeEmpty();
    }

    private static List<ValidationResult> ValidateDTO(object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, context, results, true);
        return results;
    }
}
