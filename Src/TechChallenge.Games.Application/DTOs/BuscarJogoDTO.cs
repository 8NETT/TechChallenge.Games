using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public class BuscarJogoDTO
    {
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo 50 caracteres.")]
        public string Termo { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "O campo {0} deve ser positivo.")]
        public int Inicio { get; set; } = 0;

        [Range(1, 100, ErrorMessage = "O campo {0} deve estar entre 1 e 100.")]
        public int Tamanho { get; set; } = 10;
    }
}
