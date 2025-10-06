using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public class BuscarJogoDTO : ObterTodosDto
    {
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo 50 caracteres.")]
        public string Termo { get; set; } = string.Empty;
    }
}
