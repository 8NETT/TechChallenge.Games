namespace TechChallenge.Games.Application.DTOs
{
    public sealed class BibliotecaDTO
    {
        public int UsuarioId { get; set; }
        public List<JogoDTO> Jogos { get; set; } = new();
    }
}
