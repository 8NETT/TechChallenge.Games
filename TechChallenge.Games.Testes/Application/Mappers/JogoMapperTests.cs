using FluentAssertions;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Application.Mappers;
using TechChallenge.Games.Core.Entity;
using Xunit;

namespace TechChallenge.Games.Testes.Application.Mappers;

public class JogoMapperTests
{
    [Fact]
    public void ToDTO_DeveMapearJogoParaDTO_ComTodosOsCampos()
    {
        // Arrange
        var jogo = Jogo.New()
            .Id(1)
            .Nome("Super Mario Bros")
            .Valor(59.99m)
            .Desconto(10)
            .Descricao("Jogo clássico")
            .DataCriacao(DateTime.Now)
            .Build();

        // Act
        var jogoDTO = JogoMapper.ToDTO(jogo);

        // Assert
        jogoDTO.Should().NotBeNull();
        jogoDTO.Id.Should().Be(jogo.Id);
        jogoDTO.Nome.Should().Be(jogo.Nome);
        jogoDTO.Valor.Should().Be(jogo.Valor);
        jogoDTO.Desconto.Should().Be(jogo.Desconto);
    }

    [Fact]
    public void ToDTO_DeveMapearJogoComDescricaoNula()
    {
        // Arrange
        var jogo = Jogo.New()
            .Id(2)
            .Nome("Jogo Sem Descrição")
            .Valor(29.99m)
            .Desconto(0)
            .Descricao(null)
            .DataCriacao(DateTime.Now)
            .Build();

        // Act
        var jogoDTO = JogoMapper.ToDTO(jogo);

        // Assert
        jogoDTO.Should().NotBeNull();
        jogoDTO.Id.Should().Be(2);
        jogoDTO.Nome.Should().Be("Jogo Sem Descrição");
        jogoDTO.Valor.Should().Be(29.99m);
        jogoDTO.Desconto.Should().Be(0);
    }

    [Fact]
    public void ToDTO_DeveMapearJogoComValoresExtremos()
    {
        // Arrange
        var jogo = Jogo.New()
            .Id(int.MaxValue)
            .Nome("Jogo Com Valores Extremos")
            .Valor(decimal.MaxValue)
            .Desconto(100)
            .Descricao("Descrição com valores extremos")
            .DataCriacao(DateTime.MaxValue)
            .Build();

        // Act
        var jogoDTO = JogoMapper.ToDTO(jogo);

        // Assert
        jogoDTO.Should().NotBeNull();
        jogoDTO.Id.Should().Be(int.MaxValue);
        jogoDTO.Nome.Should().Be("Jogo Com Valores Extremos");
        jogoDTO.Valor.Should().Be(decimal.MaxValue);
        jogoDTO.Desconto.Should().Be(100);
    }
    [Fact]
    public void ToEntity_DeveMapearCadastrarJogoDTOParaJogo()
    {
        // Arrange
        var cadastrarJogoDTO = new CadastrarJogoDTO
        {
            Nome = "The Legend of Zelda",
            Valor = 69.99m,
            Desconto = 15,
            Descricao = "Jogo de aventura épico"
        };

        // Act
        var jogo = JogoMapper.ToEntity(cadastrarJogoDTO);
        
        // Assert
        jogo.Should().NotBeNull();
        jogo.Nome.Should().Be(cadastrarJogoDTO.Nome);
        jogo.Valor.Should().Be(cadastrarJogoDTO.Valor);
        jogo.Desconto.Should().Be(cadastrarJogoDTO.Desconto);
        jogo.Descricao.Should().Be(cadastrarJogoDTO.Descricao);
        jogo.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ToEntity_DeveMapearCadastrarJogoDTOComDescricaoNula()
    {
        // Arrange
        var cadastrarJogoDTO = new CadastrarJogoDTO
        {
            Nome = "God of War",
            Valor = 49.99m,
            Desconto = 20,
            Descricao = null
        };

        // Act
        var jogo = JogoMapper.ToEntity(cadastrarJogoDTO);

        // Assert
        jogo.Should().NotBeNull();
        jogo.Nome.Should().Be(cadastrarJogoDTO.Nome);
        jogo.Valor.Should().Be(cadastrarJogoDTO.Valor);
        jogo.Desconto.Should().Be(cadastrarJogoDTO.Desconto);
        jogo.Descricao.Should().BeNull();
    }

    [Fact]
    public void ToEntity_DeveMapearCadastrarJogoDTOComDescricaoVazia()
    {
        // Arrange
        var cadastrarJogoDTO = new CadastrarJogoDTO
        {
            Nome = "Jogo Com Descrição Vazia",
            Valor = 19.99m,
            Desconto = 0,
            Descricao = ""
        };

        // Act
        var jogo = JogoMapper.ToEntity(cadastrarJogoDTO);

        // Assert
        jogo.Should().NotBeNull();
        jogo.Descricao.Should().Be("");
    }

    [Fact]
    public void ToEntity_DeveMapearCadastrarJogoDTOComDescontoZero()
    {
        // Arrange
        var cadastrarJogoDTO = new CadastrarJogoDTO
        {
            Nome = "Jogo Sem Desconto",
            Valor = 99.99m,
            Desconto = 0,
            Descricao = "Jogo com preço cheio"
        };

        // Act
        var jogo = JogoMapper.ToEntity(cadastrarJogoDTO);

        // Assert
        jogo.Should().NotBeNull();
        jogo.Desconto.Should().Be(0);
    }

    [Fact]
    public void ToEntity_DeveMapearCadastrarJogoDTOComDescontoMaximo()
    {
        // Arrange
        var cadastrarJogoDTO = new CadastrarJogoDTO
        {
            Nome = "Jogo Gratuito",
            Valor = 59.99m,
            Desconto = 100,
            Descricao = "Jogo com desconto total"
        };

        // Act
        var jogo = JogoMapper.ToEntity(cadastrarJogoDTO);

        // Assert
        jogo.Should().NotBeNull();
        jogo.Desconto.Should().Be(100);
    }
    
    [Fact]
    public void ToEntity_DeveMapearAlterarJogoDTOParaJogo()
    {
        // Arrange
        var jogoExistente = Jogo.New()
            .Id(1)
            .Nome("Jogo Antigo")
            .Valor(29.99m)
            .Desconto(0)
            .Descricao("Descrição antiga")
            .DataCriacao(DateTime.Now.AddDays(-30))
            .Build();

        var alterarJogoDTO = new AlterarJogoDTO
        {
            Id = 1,
            Nome = "Jogo Atualizado",
            Valor = 39.99m,
            Desconto = 25,
            Descricao = "Nova descrição"
        };

        // Act
        var jogoAtualizado = JogoMapper.ToEntity(alterarJogoDTO, jogoExistente);

        // Assert
        jogoAtualizado.Should().NotBeNull();
        jogoAtualizado.Id.Should().Be(jogoExistente.Id);
        jogoAtualizado.DataCriacao.Should().Be(jogoExistente.DataCriacao); // Deve preservar data original
        jogoAtualizado.Nome.Should().Be(alterarJogoDTO.Nome);
        jogoAtualizado.Valor.Should().Be(alterarJogoDTO.Valor);
        jogoAtualizado.Desconto.Should().Be(alterarJogoDTO.Desconto);
        jogoAtualizado.Descricao.Should().Be(alterarJogoDTO.Descricao);
    }

    [Fact]
    public void ToEntity_DevePreservarIdOriginal_MesmoComIdDiferenteNoDTO()
    {
        // Arrange
        var jogoExistente = Jogo.New()
            .Id(5)
            .Nome("Jogo Original")
            .Valor(25.99m)
            .Desconto(5)
            .Descricao("Descrição original")
            .DataCriacao(DateTime.Now.AddDays(-15))
            .Build();

        var alterarJogoDTO = new AlterarJogoDTO
        {
            Id = 999, // ID diferente do original
            Nome = "Nome Alterado",
            Valor = 35.99m,
            Desconto = 15,
            Descricao = "Descrição alterada"
        };

        // Act
        var jogoAtualizado = JogoMapper.ToEntity(alterarJogoDTO, jogoExistente);

        // Assert
        jogoAtualizado.Id.Should().Be(5); // Deve manter o ID original
        jogoAtualizado.Nome.Should().Be("Nome Alterado");
    }

    [Fact]
    public void ToEntity_DevePreservarDataCriacaoOriginal()
    {
        // Arrange
        var dataCriacaoOriginal = DateTime.Now.AddYears(-1);
        var jogoExistente = Jogo.New()
            .Id(3)
            .Nome("Jogo Antigo")
            .Valor(45.99m)
            .Desconto(10)
            .Descricao("Descrição original")
            .DataCriacao(dataCriacaoOriginal)
            .Build();

        var alterarJogoDTO = new AlterarJogoDTO
        {
            Id = 3,
            Nome = "Jogo Renovado",
            Valor = 55.99m,
            Desconto = 20,
            Descricao = "Nova vida para o jogo"
        };

        // Act
        var jogoAtualizado = JogoMapper.ToEntity(alterarJogoDTO, jogoExistente);

        // Assert
        jogoAtualizado.DataCriacao.Should().Be(dataCriacaoOriginal);
        jogoAtualizado.DataCriacao.Should().NotBeCloseTo(DateTime.Now, TimeSpan.FromDays(1));
    }

    [Fact]
    public void ToEntity_DevePermitirAlterarParaDescricaoNula()
    {
        // Arrange
        var jogoExistente = Jogo.New()
            .Id(4)
            .Nome("Jogo Com Descrição")
            .Valor(33.99m)
            .Desconto(8)
            .Descricao("Descrição que será removida")
            .DataCriacao(DateTime.Now.AddDays(-7))
            .Build();

        var alterarJogoDTO = new AlterarJogoDTO
        {
            Id = 4,
            Nome = "Jogo Sem Descrição",
            Valor = 33.99m,
            Desconto = 8,
            Descricao = null
        };

        // Act
        var jogoAtualizado = JogoMapper.ToEntity(alterarJogoDTO, jogoExistente);

        // Assert
        jogoAtualizado.Descricao.Should().BeNull();
    }
    
    [Fact]
    public void DeveManterConsistencia_NoCicloCompletoDeMapeamento()
    {
        // Arrange
        var dtoOriginal = new CadastrarJogoDTO
        {
            Nome = "Cyberpunk 2077",
            Valor = 199.99m,
            Desconto = 50,
            Descricao = "RPG futurístico"
        };

        // Act
        var entity = JogoMapper.ToEntity(dtoOriginal);
        var dtoFinal = JogoMapper.ToDTO(entity);

        // Assert
        dtoFinal.Nome.Should().Be(dtoOriginal.Nome);
        dtoFinal.Valor.Should().Be(dtoOriginal.Valor);
        dtoFinal.Desconto.Should().Be(dtoOriginal.Desconto);
    }

    [Fact]
    public void DeveManterConsistencia_NoCicloCompletoDeAlteracao()
    {
        // Arrange
        var entityOriginal = Jogo.New()
            .Id(10)
            .Nome("Jogo Original")
            .Valor(79.99m)
            .Desconto(15)
            .Descricao("Descrição original")
            .DataCriacao(DateTime.Now.AddMonths(-2))
            .Build();

        var alterarDTO = new AlterarJogoDTO
        {
            Id = 10,
            Nome = "Jogo Alterado",
            Valor = 89.99m,
            Desconto = 25,
            Descricao = "Nova descrição"
        };

        // Act
        var entityAlterada = JogoMapper.ToEntity(alterarDTO, entityOriginal);
        var dtoFinal = JogoMapper.ToDTO(entityAlterada);

        // Assert
        dtoFinal.Id.Should().Be(entityOriginal.Id);
        dtoFinal.Nome.Should().Be(alterarDTO.Nome);
        dtoFinal.Valor.Should().Be(alterarDTO.Valor);
        dtoFinal.Desconto.Should().Be(alterarDTO.Desconto);
    }
}
