using FluentAssertions;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Exceptions;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Application.Mappers;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TechChallenge.Games.Testes;

public class JogoIntegrationTests
{
    [Fact]
    public void DeveCriarJogo_ComDadosValidos()
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
        // 👇 Removido o Id, pois não foi persistido ainda
        jogo.DataCriacao.Should().BeCloseTo(dataCriacao, TimeSpan.FromSeconds(1));
    }


    [Fact]
    public void DeveLancarExcecao_QuandoDadosInvalidos()
    {
        // Act
        var act = () => Jogo.New()
            .Nome("") // Nome inválido
            .Valor(-10) // Valor inválido
            .Desconto(150) // Desconto inválido
            .DataCriacao(DateTime.Now)
            .Build();

        // Assert
        act.Should().Throw<EstadoInvalidoException>()
            .WithMessage("Não é possível criar um jogo em um estado inválido.");
    }

    [Fact]
    public void DeveValidarCadastrarJogoDTO_ComDadosValidos()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "The Legend of Zelda",
            Valor = 69.99m,
            Desconto = 15,
            Descricao = "Jogo de aventura épico"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
        dto.Nome.Should().Be("The Legend of Zelda");
        dto.Valor.Should().Be(69.99m);
        dto.Desconto.Should().Be(15);
        dto.Descricao.Should().Be("Jogo de aventura épico");
    }

    [Fact]
    public void DeveValidarAlterarJogoDTO_ComDadosValidos()
    {
        // Arrange
        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "God of War",
            Valor = 49.99m,
            Desconto = 20,
            Descricao = "Jogo de ação épico"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().BeEmpty();
        dto.Id.Should().Be(1);
        dto.Nome.Should().Be("God of War");
        dto.Valor.Should().Be(49.99m);
        dto.Desconto.Should().Be(20);
        dto.Descricao.Should().Be("Jogo de ação épico");
    }

    [Xunit.Theory]
    [InlineData("", "deve ser preenchido")]
    [InlineData("AB", "deve ter entre 3 a 100")]
    public void DeveValidarCadastrarJogoDTO_ComNomeInvalido(string nomeInvalido, string mensagemEsperada)
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = nomeInvalido,
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Descrição válida"
        };

        // Act
        var validationResults = ValidateDTO(dto);

        // Assert
        validationResults.Should().Contain(vr => vr.ErrorMessage!.Contains(mensagemEsperada));
    }
    
    [Fact]
    public void DeveConverterCadastrarJogoDTO_ParaEntity()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Final Fantasy VII",
            Valor = 79.99m,
            Desconto = 25,
            Descricao = "RPG clássico"
        };

        // Act
        var entity = JogoMapper.ToEntity(dto);

        // Assert
        entity.Should().NotBeNull();
        entity.Nome.Should().Be(dto.Nome);
        entity.Valor.Should().Be(dto.Valor);
        entity.Desconto.Should().Be(dto.Desconto);
        entity.Descricao.Should().Be(dto.Descricao);
        // 👇 Removido o Id, pois ainda não foi salvo no banco
        entity.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DeveConverterEntity_ParaJogoDTO()
    {
        // Arrange
        var entity = Jogo.New()
            .Id(1)
            .Nome("Cyberpunk 2077")
            .Valor(89.99m)
            .Desconto(30)
            .Descricao("RPG futurístico")
            .DataCriacao(DateTime.Now)
            .Build();

        // Act
        var dto = JogoMapper.ToDTO(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(entity.Id);
        dto.Nome.Should().Be(entity.Nome);
        dto.Valor.Should().Be(entity.Valor);
        dto.Desconto.Should().Be(entity.Desconto);
    }

    [Fact]
    public void DeveConverterAlterarJogoDTO_ParaEntityExistente()
    {
        // Arrange
        var entityExistente = Jogo.New()
            .Id(1)
            .Nome("Nome Antigo")
            .Valor(50.00m)
            .Desconto(0)
            .Descricao("Descrição antiga")
            .DataCriacao(DateTime.Now.AddDays(-30))
            .Build();

        var dto = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Nome Novo",
            Valor = 75.99m,
            Desconto = 15,
            Descricao = "Nova descrição"
        };

        // Act
        var entityAtualizada = JogoMapper.ToEntity(dto, entityExistente);

        // Assert
        entityAtualizada.Id.Should().Be(entityExistente.Id);
        entityAtualizada.DataCriacao.Should().Be(entityExistente.DataCriacao); // Deve manter a data original
        entityAtualizada.Nome.Should().Be(dto.Nome);
        entityAtualizada.Valor.Should().Be(dto.Valor);
        entityAtualizada.Desconto.Should().Be(dto.Desconto);
        entityAtualizada.Descricao.Should().Be(dto.Descricao);
    }

    [Fact]
    public void DevePermitirDescricaoNula_EmCadastrarJogoDTO()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Sem Descrição",
            Valor = 29.99m,
            Desconto = 5,
            Descricao = null
        };

        // Act
        var validationResults = ValidateDTO(dto);
        var entity = JogoMapper.ToEntity(dto);

        // Assert
        validationResults.Should().BeEmpty();
        entity.Descricao.Should().BeNull();
    }

    [Fact]
    public void DevePermitirDescontoZero()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Sem Desconto",
            Valor = 39.99m,
            Desconto = 0,
            Descricao = "Jogo sem promoção"
        };

        // Act
        var validationResults = ValidateDTO(dto);
        var entity = JogoMapper.ToEntity(dto);

        // Assert
        validationResults.Should().BeEmpty();
        entity.Desconto.Should().Be(0);
    }

    [Fact]
    public void DevePermitirDescontoMaximo()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Com Desconto Total",
            Valor = 99.99m,
            Desconto = 100,
            Descricao = "Jogo gratuito"
        };

        // Act
        var validationResults = ValidateDTO(dto);
        var entity = JogoMapper.ToEntity(dto);

        // Assert
        validationResults.Should().BeEmpty();
        entity.Desconto.Should().Be(100);
    }

    private static List<ValidationResult> ValidateDTO(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }
}