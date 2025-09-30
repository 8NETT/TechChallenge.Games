using FluentValidation.Results;
using TechChallenge.Games.Command.Domain.Aggregates;
using TechChallenge.Games.Command.Domain.Events;
using TechChallenge.Games.Command.Domain.Exceptions;
using TechChallenge.Games.Command.Domain.Extensions;
using TechChallenge.Games.Command.Domain.Validators;

namespace TechChallenge.Games.Command.Domain.Builders
{
    public sealed class JogoBuilder
    {
        private Jogo _jogo = new Jogo();

        public JogoBuilder Nome(string nome) => this.Tee(b => b._jogo.Nome = nome);
        public JogoBuilder Descricao(string? descricao) => this.Tee(b => b._jogo.Descricao = descricao);
        public JogoBuilder DataLancamento(DateTime dataLancamento) => this.Tee(b => b._jogo.DataLancamento = dataLancamento);
        public JogoBuilder Preco(decimal preco) => this.Tee(b => b._jogo.Preco = preco);

        public ValidationResult Validate() =>
            new JogoValidator().Validate(_jogo);

        public Jogo Build()
        {
            if (!Validate().IsValid)
                throw new EstadoInvalidoException("Não é possível criar um jogo em um estado inválido.");

            _jogo.RaiseEvent(new JogoCriadoEvent
            {
                AggregateId = Guid.NewGuid(),
                Nome = _jogo.Nome,
                Preco = _jogo.Preco,
                DataLancamento = _jogo.DataLancamento
            });

            return _jogo;
        }
    }
}
