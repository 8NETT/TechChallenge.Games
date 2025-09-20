using TechChallenge.Games.Core.Builders;

namespace TechChallenge.Games.Core.Entity;

public class Jogo : EntityBase
{
    public string Nome { get; protected internal set; }
    public string? Descricao { get; protected internal set; }
    public decimal Valor { get; protected internal set; }
    public int Desconto { get; protected internal set; }

    protected internal Jogo() { }

    public static JogoBuilder New() => new JogoBuilder();
}