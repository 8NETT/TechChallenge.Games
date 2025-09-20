using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public class CadastrarJogoDTO
    {
        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Length(3, 100, ErrorMessage = "O campo {0} deve ter entre 3 a 100 caracteres.")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Range(0, int.MaxValue, ErrorMessage = "O campo {0} deve ser positivo.")]
        public required decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Range(0, 100, ErrorMessage = "O campo {0} deve estar entre 0 e 100.")]
        public required int Desconto { get; set; }

        [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }
    }
}
