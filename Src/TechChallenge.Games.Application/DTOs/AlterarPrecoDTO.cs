using System.ComponentModel.DataAnnotations;

namespace TechChallenge.Games.Application.DTOs
{
    public sealed class AlterarPrecoDTO
    {
        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        public required Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} deve ser preenchido.")]
        [Range(0, int.MaxValue, ErrorMessage = "O campo {0} deve ser positivo.")]
        public required decimal NovoPreco { get; set; }
    }
}
