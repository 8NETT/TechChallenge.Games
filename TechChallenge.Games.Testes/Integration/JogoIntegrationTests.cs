using Ardalis.Result;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TechChallenge.Games.Application.Services;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Repository;
using TechChallenge.Games.Core.Exceptions;
using Xunit;

namespace TechChallenge.Games.Testes.Integration;

public class JogoIntegrationTests : IDisposable
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IJogoRepository> _mockJogoRepository;
    private readonly JogoService _jogoService;
    private readonly List<Jogo> _jogosInMemory;

    public JogoIntegrationTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockJogoRepository = new Mock<IJogoRepository>();
        _jogosInMemory = new List<Jogo>();

        ConfigurarMocks();

        _mockUnitOfWork.Setup(u => u.JogoRepository).Returns(_mockJogoRepository.Object);
        _jogoService = new JogoService(_mockUnitOfWork.Object);
    }

    private void ConfigurarMocks()
    {
        // Setup para ObterTodosAsync
        _mockJogoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(() => _jogosInMemory.AsEnumerable());

        // Setup para ObterPorIdAsync
        _mockJogoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => _jogosInMemory.FirstOrDefault(j => j.Id == id));

        // Setup para ObterPorNomeAsync
        _mockJogoRepository.Setup(r => r.ObterPorNomeAsync(It.IsAny<string>()))
            .ReturnsAsync((string nome) => _jogosInMemory.FirstOrDefault(j => j.Nome == nome));

        // Setup para Cadastrar
        _mockJogoRepository.Setup(r => r.Cadastrar(It.IsAny<Jogo>()))
            .Callback<Jogo>(jogo => _jogosInMemory.Add(jogo));

        // Setup para Deletar
        _mockJogoRepository.Setup(r => r.Deletar(It.IsAny<Jogo>()))
            .Callback<Jogo>(jogo => _jogosInMemory.Remove(jogo));

        // Setup para CommitAsync
        _mockUnitOfWork.Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task FluxoCompleto_DeveExecutarCRUDComSucesso()
    {
        // 1. CREATE - Cadastrar um jogo
        var cadastrarDto = new CadastrarJogoDTO
        {
            Nome = "Super Mario Bros",
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Jogo clássico da Nintendo"
        };

        var resultadoCadastro = await _jogoService.CadastrarAsync(cadastrarDto);

        resultadoCadastro.IsSuccess.Should().BeTrue();
        var jogoId = resultadoCadastro.Value.Id;

        // 2. READ - Obter o jogo cadastrado
        var resultadoLeitura = await _jogoService.ObterPorIdAsync(jogoId);

        resultadoLeitura.IsSuccess.Should().BeTrue();
        resultadoLeitura.Value.Nome.Should().Be("Super Mario Bros");

        // 3. UPDATE - Alterar o jogo
        var alterarDto = new AlterarJogoDTO
        {
            Id = jogoId,
            Nome = "Super Mario Bros - Edição Especial",
            Valor = 69.99m,
            Desconto = 15,
            Descricao = "Versão especial com conteúdo extra"
        };

        var resultadoAlteracao = await _jogoService.AlterarAsync(alterarDto);

        resultadoAlteracao.IsSuccess.Should().BeTrue();
        resultadoAlteracao.Value.Nome.Should().Be("Super Mario Bros - Edição Especial");

        // 4. DELETE - Deletar o jogo
        var resultadoDelecao = await _jogoService.DeletarAsync(jogoId);

        resultadoDelecao.IsSuccess.Should().BeTrue();

        // 5. Verificar que foi deletado
        var resultadoVerificacao = await _jogoService.ObterPorIdAsync(jogoId);
        resultadoVerificacao.IsNotFound().Should().BeFalse();
    }

    [Fact]
    public async Task DeveBuscarTodosOsJogos_QuandoRepositorioTemDados()
    {
        // Arrange - Adicionar jogos diretamente na lista in-memory
        var jogo1 = Jogo.New().Id(1).Nome("Jogo 1").Valor(29.99m).Desconto(0).DataCriacao(DateTime.Now).Build();
        var jogo2 = Jogo.New().Id(2).Nome("Jogo 2").Valor(39.99m).Desconto(10).DataCriacao(DateTime.Now).Build();

        _jogosInMemory.AddRange(new[] { jogo1, jogo2 });

        // Act
        var resultado = await _jogoService.ObterTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(j => j.Nome == "Jogo 1");
        resultado.Should().Contain(j => j.Nome == "Jogo 2");
    }

    [Fact]
    public async Task DeveRetornarListaVazia_QuandoRepositorioVazio()
    {
        // Act
        var resultado = await _jogoService.ObterTodosAsync();

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task DeveImpedirCadastroComNomesDuplicados()
    {
        // Arrange - Cadastrar primeiro jogo
        var primeiroJogo = new CadastrarJogoDTO
        {
            Nome = "Jogo Único",
            Valor = 49.99m,
            Desconto = 5,
            Descricao = "Primeiro jogo"
        };

        await _jogoService.CadastrarAsync(primeiroJogo);

        // Act - Tentar cadastrar segundo jogo com mesmo nome
        var segundoJogo = new CadastrarJogoDTO
        {
            Nome = "Jogo Único", // Mesmo nome
            Valor = 59.99m,
            Desconto = 10,
            Descricao = "Segundo jogo"
        };

        var resultado = await _jogoService.CadastrarAsync(segundoJogo);

        // Assert
        resultado.IsConflict().Should().BeTrue();
        resultado.Errors.Should().Contain(e => e.Contains("Já existe um jogo cadastrado com esse nome"));
    }

    [Fact]
    public async Task DevePermitirAlteracaoSemMudarNome()
    {
        // Arrange - Cadastrar jogo inicial
        var cadastrarDto = new CadastrarJogoDTO
        {
            Nome = "Jogo Original",
            Valor = 29.99m,
            Desconto = 0,
            Descricao = "Descrição original"
        };

        var resultadoCadastro = await _jogoService.CadastrarAsync(cadastrarDto);
        var jogoId = resultadoCadastro.Value.Id;

        // Act - Alterar sem mudar o nome
        var alterarDto = new AlterarJogoDTO
        {
            Id = jogoId,
            Nome = "Jogo Original", // Mesmo nome
            Valor = 39.99m, // Valor alterado
            Desconto = 15, // Desconto alterado
            Descricao = "Nova descrição"
        };

        var resultadoAlteracao = await _jogoService.AlterarAsync(alterarDto);

        // Assert
        resultadoAlteracao.IsSuccess.Should().BeTrue();
        resultadoAlteracao.Value.Nome.Should().Be("Jogo Original");
        resultadoAlteracao.Value.Valor.Should().Be(39.99m);
    }

    [Fact]
    public async Task DeveRejeitarCadastroComDadosInvalidos()
    {
        // Arrange
        var dtoInvalido = new CadastrarJogoDTO
        {
            Nome = "", // Nome inválido
            Valor = -10m, // Valor inválido
            Desconto = 150, // Desconto inválido
            Descricao = new string('A', 501) // Descrição muito longa
        };

        // Act
        var resultado = await _jogoService.CadastrarAsync(dtoInvalido);

        // Assert
        resultado.IsInvalid().Should().BeTrue();
        resultado.ValidationErrors.Should().HaveCountGreaterThan(0);
        _jogosInMemory.Should().BeEmpty(); // Nenhum jogo deve ter sido adicionado
    }

    [Fact]
    public async Task DeveRejeitarAlteracaoComDadosInvalidos()
    {
        // Arrange - Primeiro cadastrar um jogo válido
        var cadastrarDto = new CadastrarJogoDTO
        {
            Nome = "Jogo Válido",
            Valor = 49.99m,
            Desconto = 10,
            Descricao = "Jogo para teste"
        };

        var resultadoCadastro = await _jogoService.CadastrarAsync(cadastrarDto);
        var jogoId = resultadoCadastro.Value.Id;

        // Act - Tentar alterar com dados inválidos
        var alterarDtoInvalido = new AlterarJogoDTO
        {
            Id = jogoId,
            Nome = "AB", // Nome muito curto
            Valor = 0m, // Valor inválido
            Desconto = -5, // Desconto inválido
            Descricao = "Tentativa inválida"
        };

        var resultado = await _jogoService.AlterarAsync(alterarDtoInvalido);

        // Assert
        resultado.IsInvalid().Should().BeTrue();
        resultado.ValidationErrors.Should().HaveCountGreaterThan(0);

        // O jogo original deve permanecer inalterado
        var jogoOriginal = await _jogoService.ObterPorIdAsync(jogoId);
        jogoOriginal.Value.Nome.Should().Be("Jogo Válido");
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoBuscarJogoInexistente()
    {
        // Act
        var resultado = await _jogoService.ObterPorIdAsync(999);

        // Assert
        resultado.IsNotFound().Should().BeTrue();
        resultado.Errors.Should().Contain(e => e.Contains("não localizado"));
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoTentarAlterarJogoInexistente()
    {
        // Arrange
        var alterarDto = new AlterarJogoDTO
        {
            Id = 999,
            Nome = "Jogo Inexistente",
            Valor = 49.99m,
            Desconto = 10,
            Descricao = "Tentativa de alterar jogo que não existe"
        };

        // Act
        var resultado = await _jogoService.AlterarAsync(alterarDto);

        // Assert
        resultado.IsNotFound().Should().BeTrue();
        resultado.Errors.Should().Contain(e => e.Contains("não localizado"));
    }

    [Fact]
    public async Task DeveRetornarNotFound_QuandoTentarDeletarJogoInexistente()
    {
        // Act
        var resultado = await _jogoService.DeletarAsync(999);

        // Assert
        resultado.IsNotFound().Should().BeTrue();
        resultado.Errors.Should().Contain(e => e.Contains("não localizado"));
    }

    [Fact]
    public async Task DeveManterEstadoConsistente_AposOperacoesConcorrentes()
    {
        // Arrange - Simular múltiplas operações
        var dto1 = new CadastrarJogoDTO { Nome = "Jogo 1", Valor = 29.99m, Desconto = 0, Descricao = "Primeiro" };
        var dto2 = new CadastrarJogoDTO { Nome = "Jogo 2", Valor = 39.99m, Desconto = 10, Descricao = "Segundo" };
        var dto3 = new CadastrarJogoDTO { Nome = "Jogo 3", Valor = 49.99m, Desconto = 20, Descricao = "Terceiro" };

        // Act - Executar operações em paralelo
        var tasks = new[]
        {
            _jogoService.CadastrarAsync(dto1),
            _jogoService.CadastrarAsync(dto2),
            _jogoService.CadastrarAsync(dto3)
        };

        var resultados = await Task.WhenAll(tasks);

        // Assert - Todos devem ter sucesso
        resultados.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        _jogosInMemory.Should().HaveCount(3);

        // Verificar que todos os jogos estão acessíveis
        var todosJogos = await _jogoService.ObterTodosAsync();
        todosJogos.Should().HaveCount(3);
    }

    public void Dispose()
    {
        _jogoService?.Dispose();
        _jogosInMemory.Clear();
    }
}