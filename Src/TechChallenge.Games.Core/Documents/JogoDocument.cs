namespace TechChallenge.Games.Core.Documents;

public class JogoDocument
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public double Valor { get; set; }
    public int Desconto { get; set; }
    public DateTime DataCriacao { get; set; }
    public string? Descricao { get; set; }
    
    public int Popularidade { get; set; }
    public IEnumerable<string>? Keywords { get; set; }
}