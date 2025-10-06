using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public class AlterarDadosDTO
    {
        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        public required Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Length(3, 100, ErrorMessage = "O campo {0} deve ter entre 3 a 100 caracteres.")]
        public string? Nome { get; set; }

        [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        public DateTime? DataLancamento { get; set; }
    }
}
