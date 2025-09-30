using FluentValidation;
using TechChallenge.Games.Command.Domain.Aggregates;

namespace TechChallenge.Games.Command.Domain.Validators
{
    internal sealed class JogoValidator : AbstractValidator<Jogo>
    {
        public JogoValidator()
        {
            RuleFor(j => j.Nome)
                .NotEmpty().WithMessage("O nome deve ser preenchido.")
                .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

            RuleFor(j => j.Descricao)
                .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

            RuleFor(j => j.Preco)
                .NotEmpty().WithMessage("O preço deve ser preenchido.")
                .GreaterThan(0M).WithMessage("O preço deve ser um valor positivo.");

            RuleFor(j => j.DataLancamento)
                .NotEmpty().WithMessage("A data de lançamento deve ser preenchida.");
        }
    }
}
