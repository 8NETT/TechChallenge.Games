using FluentAssertions;
using Moq;
using Ardalis.Result;
using TechChallenge.Games.Application.Services;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Repository;
using Xunit;

namespace TechChallenge.Games.Testes.Application.Services;

public class JogoServiceTestsSimplified : IDisposable
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IJogoRepository> _mockJogoRepository;
    private readonly JogoService _jogoService;

    public JogoServiceTestsSimplified()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockJogoRepository = new Mock<IJogoRepository>();
        
        _mockUnitOfWork.Setup(u => u.JogoRepository).Returns(_mockJogoRepository.Object);
        
        _jogoService = new JogoService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoHouverJogos()
    {
        // Arrange
        _mockJogoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(new List<Jogo>());

        // Act
        var resultado = await _jogoService.ObterTodosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarJogos_QuandoExistiremJogos()
    {
        // Arrange
        var jogos = new List<Jogo>
        {
            Jogo.New().Id(1).Nome("Jogo 1").Valor(10.99m).Desconto(0).DataCriacao(DateTime.Now).Build(),
            Jogo.New().Id(2).Nome("Jogo 2").Valor(20.99m).Desconto(10).DataCriacao(DateTime.Now).Build()
        };

        _mockJogoRepository.Setup(r => r.ObterTodosAsync())
            .ReturnsAsync(jogos);

        // Act
        var resultado = await _jogoService.ObterTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().AllSatisfy(dto =>
        {
            dto.Should().NotBeNull();
            dto.Id.Should().BeGreaterThan(0);
            dto.Nome.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNotFound_QuandoJogoNaoExistir()
    {
        // Arrange
        var id = 999;
        _mockJogoRepository.Setup(r => r.ObterPorIdAsync(id))
            .ReturnsAsync((Jogo?)null);

        // Act
        var resultado = await _jogoService.ObterPorIdAsync(id);

        // Assert
        resultado.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarJogo_QuandoJogoExistir()
    {
        // Arrange
        var id = 1;
        var jogo = Jogo.New()
            .Id(id)
            .Nome("Super Mario Bros")
            .Valor(59.99m)
            .Desconto(10)
            .DataCriacao(DateTime.Now)
            .Build();

        _mockJogoRepository.Setup(r => r.ObterPorIdAsync(id))
            .ReturnsAsync(jogo);

        // Act
        var resultado = await _jogoService.ObterPorIdAsync(id);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Id.Should().Be(id);
        resultado.Value.Nome.Should().Be(jogo.Nome);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarConflict_QuandoJogoComMesmoNomeExistir()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Jogo Existente",
            Valor = 59.99m,
            Desconto = 0,
            Descricao = "Descrição"
        };

        var jogoExistente = Jogo.New()
            .Id(1)
            .Nome(dto.Nome)
            .Valor(39.99m)
            .Desconto(5)
            .DataCriacao(DateTime.Now.AddDays(-10))
            .Build();

        _mockJogoRepository.Setup(r => r.ObterPorNomeAsync(dto.Nome))
            .ReturnsAsync(jogoExistente);

        // Act
        var resultado = await _jogoService.CadastrarAsync(dto);

        // Assert
        resultado.Status.Should().Be(ResultStatus.Conflict);
        _mockJogoRepository.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DeveRetornarSucesso_QuandoDadosValidosENomeUnico()
    {
        // Arrange
        var dto = new CadastrarJogoDTO
        {
            Nome = "Novo Jogo",
            Valor = 79.99m,
            Desconto = 15,
            Descricao = "Descrição do novo jogo"
        };

        _mockJogoRepository.Setup(r => r.ObterPorNomeAsync(dto.Nome))
            .ReturnsAsync((Jogo?)null);

        // Act
        var resultado = await _jogoService.CadastrarAsync(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Nome.Should().Be(dto.Nome);
        resultado.Value.Valor.Should().Be(dto.Valor);
        resultado.Value.Desconto.Should().Be(dto.Desconto);
        
        _mockJogoRepository.Verify(r => r.Cadastrar(It.IsAny<Jogo>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletarAsync_DeveRetornarNotFound_QuandoJogoNaoExistir()
    {
        // Arrange
        var id = 999;
        _mockJogoRepository.Setup(r => r.ObterPorIdAsync(id))
            .ReturnsAsync((Jogo?)null);

        // Act
        var resultado = await _jogoService.DeletarAsync(id);

        // Assert
        resultado.Status.Should().Be(ResultStatus.NotFound);
        _mockJogoRepository.Verify(r => r.Deletar(It.IsAny<Jogo>()), Times.Never);
    }

    [Fact]
    public async Task DeletarAsync_DeveRetornarSucesso_QuandoJogoExistir()
    {
        // Arrange
        var id = 1;
        var jogo = Jogo.New()
            .Id(id)
            .Nome("Jogo a ser deletado")
            .Valor(29.99m)
            .Desconto(0)
            .DataCriacao(DateTime.Now.AddDays(-10))
            .Build();

        _mockJogoRepository.Setup(r => r.ObterPorIdAsync(id))
            .ReturnsAsync(jogo);

        // Act
        var resultado = await _jogoService.DeletarAsync(id);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        _mockJogoRepository.Verify(r => r.Deletar(jogo), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    public void Dispose()
    {
        _jogoService?.Dispose();
    }
}
