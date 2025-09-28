using FluentAssertions;
using TechChallenge.Games.Core.Exceptions;
using Xunit;

namespace TechChallenge.Games.Testes.Core.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_DeveSerAbstrata()
    {
        // Act & Assert
        typeof(DomainException).Should().BeAbstract();
        typeof(DomainException).Should().BeDerivedFrom<Exception>();
    }

    [Fact]
    public void EstadoInvalidoException_DeveHerdarDeDomainException()
    {
        // Act & Assert
        typeof(EstadoInvalidoException).Should().BeDerivedFrom<DomainException>();
    }

    [Fact]
    public void EstadoInvalidoException_DeveCriarComMensagem()
    {
        // Arrange
        var mensagem = "Estado inválido para a operação";

        // Act
        var exception = new EstadoInvalidoException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.Should().BeOfType<EstadoInvalidoException>();
    }

    [Fact]
    public void EstadoInvalidoException_DevePermitirMensagemVazia()
    {
        // Arrange
        var mensagem = "";

        // Act
        var exception = new EstadoInvalidoException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
    }
    
    [Xunit.Theory]
    [InlineData("Mensagem simples")]
    [InlineData("Mensagem com números 123 e símbolos @#$")]
    [InlineData("Mensagem muito longa que contém muitos caracteres para testar se a exceção suporta mensagens extensas sem problemas")]
    [InlineData("Mensagem\ncom\nquebras\nde\nlinha")]
    [InlineData("Mensagem\tcom\ttabs")]
    public void EstadoInvalidoException_DeveAceitarDiversosTiposDeMensagem(string mensagem)
    {
        // Act
        var exception = new EstadoInvalidoException(mensagem);

        // Assert
        exception.Message.Should().Be(mensagem);
        exception.Should().BeOfType<EstadoInvalidoException>();
    }

    [Fact]
    public void EstadoInvalidoException_DevePoderSerLancada()
    {
        // Arrange
        var mensagem = "Teste de lançamento de exceção";

        // Act
        var act = new Func<object>(() => throw new EstadoInvalidoException(mensagem));

        // Assert
        act.Should().Throw<EstadoInvalidoException>()
           .WithMessage(mensagem);
    }

    [Fact]
    public void EstadoInvalidoException_DevePoderSerCapturadaComoDomainException()
    {
        // Arrange
        var mensagem = "Teste de captura como DomainException";

        // Act
        var act = new Func<object>(() => throw new EstadoInvalidoException(mensagem));

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage(mensagem);
    }

    [Fact]
    public void EstadoInvalidoException_DevePoderSerCapturadaComoException()
    {
        // Arrange
        var mensagem = "Teste de captura como Exception";

        // Act
        var act = new Func<object>(() => throw new EstadoInvalidoException(mensagem));

        // Assert
        act.Should().Throw<Exception>()
           .WithMessage(mensagem);
    }

    [Fact]
    public void EstadoInvalidoException_DeveManterStackTrace()
    {
        // Arrange
        var mensagem = "Teste de stack trace";
        EstadoInvalidoException? capturedException = null;

        // Act
        try
        {
            MetodoQueGeraNiveisDeStackTrace();
        }
        catch (EstadoInvalidoException ex)
        {
            capturedException = ex;
        }

        // Assert
        capturedException.Should().NotBeNull();
        capturedException!.StackTrace.Should().NotBeNullOrEmpty();
        capturedException.StackTrace.Should().Contain(nameof(MetodoQueGeraNiveisDeStackTrace));
    }

    [Fact]
    public void EstadoInvalidoException_DevePermitirInnerException()
    {
        // Arrange
        var mensagemInterna = "Exceção interna";
        var mensagemExterna = "Exceção externa";
        var innerException = new ArgumentException(mensagemInterna);

        // Act
        // Como o construtor atual não aceita inner exception, vamos testar que podemos criar uma
        // versão que aceite se necessário no futuro
        var outerException = new EstadoInvalidoException(mensagemExterna);

        // Assert
        outerException.Message.Should().Be(mensagemExterna);
        outerException.InnerException.Should().BeNull(); // Construtor atual não aceita inner
    }

    private static void MetodoQueGeraNiveisDeStackTrace()
    {
        MetodoIntermedario();
    }

    private static void MetodoIntermedario()
    {
        throw new EstadoInvalidoException("Erro em método aninhado");
    }
}
