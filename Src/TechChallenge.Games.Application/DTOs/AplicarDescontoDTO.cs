using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public sealed class AplicarDescontoDTO
    {
        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        public required Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Range(0, 100, ErrorMessage = "O campo {0} deve estar entre 0 e 100.")]
        public required int Desconto { get; set; }
    }
}
