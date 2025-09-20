using TechChallenge.Games.Core.Entity;
using FluentValidation;

namespace TechChallenge.Games.Core.Validators
{
    internal class JogoValidator : AbstractValidator<Jogo>
    {
        public JogoValidator()
        {
            RuleFor(j => j.DataCriacao)
                .NotEmpty();

            RuleFor(j => j.Nome)
                .NotEmpty().WithMessage("O nome deve ser preenchido.")
                .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.");

            RuleFor(j => j.Valor)
                .NotEmpty().WithMessage("O preço deve ser preenchido.")
                .GreaterThan(0M).WithMessage("O preço deve ser um valor positivo.");

            RuleFor(j => j.Desconto)
                .GreaterThanOrEqualTo(0).WithMessage("O desconto deve ser maior ou igual a zero.")
                .LessThanOrEqualTo(100).WithMessage("O desconto deve ser maior ou igual a cem.");
        }
    }
}
