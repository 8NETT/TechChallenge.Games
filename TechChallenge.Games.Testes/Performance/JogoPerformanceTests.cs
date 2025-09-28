using FluentAssertions;
using System.Diagnostics;
using TechChallenge.Games.Application.DTOs;
using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Builders;
using Xunit;

namespace TechChallenge.Games.Testes.Performance;

public class JogoPerformanceTests
{
    private readonly TimeSpan _tempoLimiteOperacaoRapida = TimeSpan.FromMilliseconds(10);
    private readonly TimeSpan _tempoLimiteOperacaoNormal = TimeSpan.FromMilliseconds(100);
    private readonly TimeSpan _tempoLimiteOperacaoLenta = TimeSpan.FromMilliseconds(1000);
    
    [Fact]
    public void CriacaoJogoUnico_DeveSerMuitoRapida()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var jogo = Jogo.New()
            .Nome("Super Mario Bros")
            .Valor(59.99m)
            .Desconto(10)
            .Descricao("Jogo clássico da Nintendo")
            .DataCriacao(DateTime.Now)
            .Build();

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoRapida);
        jogo.Should().NotBeNull();
    }
    
    [Fact]
    public void CriacaoCadastrarJogoDTO_DeveSerRapida()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var dtos = new List<CadastrarJogoDTO>();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            dtos.Add(new CadastrarJogoDTO
            {
                Nome = $"Jogo Performance {i}",
                Valor = (i % 1000) + 0.99m,
                Desconto = i % 101,
                Descricao = $"Descrição de performance para o jogo número {i}"
            });
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoLenta);
        dtos.Should().HaveCount(10000);
        Console.WriteLine($"Criação de 10000 DTOs levou: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void CriacaoAlterarJogoDTO_DeveSerRapida()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var dtos = new List<AlterarJogoDTO>();

        // Act
        for (int i = 1; i <= 5000; i++)
        {
            dtos.Add(new AlterarJogoDTO
            {
                Id = i,
                Nome = $"Jogo Alterado {i}",
                Valor = (i % 500) + 19.99m,
                Desconto = i % 51,
                Descricao = $"Nova descrição para o jogo {i}"
            });
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoLenta);
        dtos.Should().HaveCount(5000);
        Console.WriteLine($"Criação de 5000 AlterarJogoDTO levou: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Fact]
    public void ValidacaoJogo_DeveSerRapida()
    {
        // Arrange
        var builder = Jogo.New()
            .Nome("Jogo Para Validação")
            .Valor(49.99m)
            .Desconto(15)
            .Descricao("Jogo para teste de performance de validação")
            .DataCriacao(DateTime.Now);

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var validationResult = builder.Validate();
            validationResult.IsValid.Should().BeTrue();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoNormal);
        Console.WriteLine($"1000 validações levaram: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void ValidacaoJogoInvalido_DeveSerRapida()
    {
        // Arrange
        var builder = Jogo.New()
            .Nome("") // Nome inválido
            .Valor(-10m) // Valor inválido
            .Desconto(150); // Desconto inválido

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var validationResult = builder.Validate();
            validationResult.IsValid.Should().BeFalse();
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoNormal);
        Console.WriteLine($"1000 validações com erros levaram: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Fact]
    public void MapeamentoEntityParaDTO_DeveSerRapido()
    {
        // Arrange
        var jogos = new List<Jogo>();
        for (int i = 1; i <= 1000; i++)
        {
            jogos.Add(Jogo.New()
                .Id(i)
                .Nome($"Jogo Mapping {i}")
                .Valor(i * 5.99m)
                .Desconto(i % 31)
                .Descricao($"Descrição {i}")
                .DataCriacao(DateTime.Now.AddDays(-i))
                .Build());
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var dtos = jogos.Select(j => new JogoDTO
        {
            Id = j.Id,
            Nome = j.Nome,
            Valor = j.Valor,
            Desconto = j.Desconto
        }).ToList();

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoNormal);
        dtos.Should().HaveCount(1000);
        Console.WriteLine($"Mapeamento de 1000 entities para DTOs levou: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void MapeamentoDTOParaEntity_DeveSerRapido()
    {
        // Arrange
        var dtos = new List<CadastrarJogoDTO>();
        for (int i = 1; i <= 500; i++)
        {
            dtos.Add(new CadastrarJogoDTO
            {
                Nome = $"Jogo DTO Mapping {i}",
                Valor = i * 12.99m,
                Desconto = i % 26,
                Descricao = $"Descrição DTO {i}"
            });
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var entities = new List<Jogo>();
        foreach (var dto in dtos)
        {
            entities.Add(Jogo.New()
                .Nome(dto.Nome)
                .Valor(dto.Valor)
                .Desconto(dto.Desconto)
                .Descricao(dto.Descricao)
                .DataCriacao(DateTime.Now)
                .Build());
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoLenta);
        entities.Should().HaveCount(500);
        Console.WriteLine($"Mapeamento de 500 DTOs para entities levou: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Fact]
    public void BuscaPorNome_EmListaGrande_DeveSerRapida()
    {
        // Arrange
        var jogos = new List<Jogo>();
        for (int i = 1; i <= 10000; i++)
        {
            jogos.Add(Jogo.New()
                .Id(i)
                .Nome($"Jogo Busca {i:D5}")
                .Valor(i % 200 + 9.99m)
                .Desconto(i % 51)
                .Descricao($"Jogo para teste de busca número {i}")
                .DataCriacao(DateTime.Now.AddDays(-i % 365))
                .Build());
        }

        var nomeProcurado = "Jogo Busca 05000";
        var stopwatch = Stopwatch.StartNew();

        // Act
        var jogoEncontrado = jogos.FirstOrDefault(j => j.Nome == nomeProcurado);

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoRapida);
        jogoEncontrado.Should().NotBeNull();
        jogoEncontrado!.Nome.Should().Be(nomeProcurado);
        Console.WriteLine($"Busca por nome em 10000 jogos levou: {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void FiltrosPorValor_EmListaGrande_DeveSerRapido()
    {
        // Arrange
        var jogos = new List<Jogo>();
        for (int i = 1; i <= 5000; i++)
        {
            jogos.Add(Jogo.New()
                .Id(i)
                .Nome($"Jogo Filtro {i}")
                .Valor(i % 100 + 10.99m)
                .Desconto(i % 76)
                .Descricao($"Jogo {i} para filtros")
                .DataCriacao(DateTime.Now.AddDays(-i % 180))
                .Build());
        }

        var stopwatch = Stopwatch.StartNew();

        // Act - Filtrar jogos com valor entre 50 e 60 reais
        var jogosFiltrados = jogos
            .Where(j => j.Valor >= 50m && j.Valor <= 60m)
            .OrderBy(j => j.Valor)
            .ToList();

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoNormal);
        jogosFiltrados.Should().NotBeEmpty();
        Console.WriteLine($"Filtro por valor em 5000 jogos retornou {jogosFiltrados.Count} resultados em {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Fact]
    public void OperacoesEmLote_DevemSerEficientes()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var operacoes = new List<string>();

        // Act
        for (int lote = 1; lote <= 10; lote++)
        {
            var jogosLote = new List<Jogo>();
            
            // Criar lote
            for (int i = 1; i <= 100; i++)
            {
                jogosLote.Add(Jogo.New()
                    .Id((lote - 1) * 100 + i)
                    .Nome($"Jogo Lote {lote}-{i:D2}")
                    .Valor((i % 50) + 15.99m)
                    .Desconto(i % 31)
                    .Descricao($"Jogo do lote {lote}, item {i}")
                    .DataCriacao(DateTime.Now)
                    .Build());
            }

            // Simular operações no lote
            var jogosComDesconto = jogosLote.Where(j => j.Desconto > 0).ToList();
            var valorTotal = jogosLote.Sum(j => j.Valor);
            var jogoMaisCaro = jogosLote.MaxBy(j => j.Valor);

            operacoes.Add($"Lote {lote}: {jogosComDesconto.Count} com desconto, valor total {valorTotal:C}");
        }

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(_tempoLimiteOperacaoLenta);
        operacoes.Should().HaveCount(10);
        Console.WriteLine($"10 lotes de 100 jogos (1000 total) processados em: {stopwatch.ElapsedMilliseconds}ms");
    }
    
    [Fact]
    public void StressTest_CriacaoMassivaDeJogos()
    {
        // Arrange
        var numeroJogos = 50000;
        var stopwatch = Stopwatch.StartNew();
        var jogos = new List<Jogo>(numeroJogos);

        // Act
        Parallel.For(0, numeroJogos, i =>
        {
            var jogo = Jogo.New()
                .Id(i + 1)
                .Nome($"Stress Game {i:D6}")
                .Valor((i % 200) + 5.99m)
                .Desconto(i % 101)
                .Descricao($"Jogo criado no stress test, número {i}")
                .DataCriacao(DateTime.Now.AddSeconds(-i % 86400))
                .Build();

            lock (jogos)
            {
                jogos.Add(jogo);
            }
        });

        stopwatch.Stop();

        // Assert
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(10)); // 10 segundos max
        jogos.Should().HaveCount(numeroJogos);
        Console.WriteLine($"Stress test: {numeroJogos} jogos criados em paralelo em {stopwatch.ElapsedMilliseconds}ms");
    }

    [Xunit.Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(5000)]
    public void StressTest_ValidacaoEmLote(int quantidade)
    {
        // Arrange
        var jogos = new List<JogoBuilder>();
        for (int i = 0; i < quantidade; i++)
        {
            jogos.Add(Jogo.New()
                .Nome($"Validation Game {i}")
                .Valor((i % 100) + 9.99m)
                .Desconto(i % 51)
                .DataCriacao(DateTime.Now));
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var resultadosValidacao = jogos.Select(j => j.Validate()).ToList();

        stopwatch.Stop();

        // Assert
        var tempoLimite = TimeSpan.FromMilliseconds(quantidade * 0.1); // 0.1ms por validação
        stopwatch.Elapsed.Should().BeLessThan(tempoLimite);
        resultadosValidacao.Should().AllSatisfy(r => r.IsValid.Should().BeTrue());
        Console.WriteLine($"Validação de {quantidade} jogos levou: {stopwatch.ElapsedMilliseconds}ms");
    }
}
