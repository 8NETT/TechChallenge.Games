namespace TechChallenge.Games.Query.Domain.Documents
{
    public class BibliotecaDocument
    {
        public int UsuarioId { get; set; }
        public List<Guid> Jogos { get; set; } = new();
    }
}
