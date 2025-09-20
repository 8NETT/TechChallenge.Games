using TechChallenge.Games.Core.Entity;
using TechChallenge.Games.Core.Exceptions;
using TechChallenge.Games.Core.Extensions;
using TechChallenge.Games.Core.Validators;
using FluentValidation.Results;

namespace TechChallenge.Games.Core.Builders
{
    public sealed class JogoBuilder
    {
        private Jogo _jogo = new Jogo();

        public JogoBuilder Id(int id) => this.Tee(b => b._jogo.Id = id);
        public JogoBuilder DataCriacao(DateTime data) => this.Tee(b => b._jogo.DataCriacao = data);
        public JogoBuilder Nome(string nome) => this.Tee(b => b._jogo.Nome = nome);
        public JogoBuilder Valor(decimal preco) => this.Tee(b => b._jogo.Valor = preco);
        public JogoBuilder Descricao(string? descricao) => this.Tee(b => b._jogo.Descricao = descricao);
        public JogoBuilder Desconto(int desconto) => this.Tee(b => b._jogo.Desconto = desconto);

        public ValidationResult Validate() =>
            new JogoValidator().Validate(_jogo);

        public Jogo Build()
        {
            if (!Validate().IsValid)
                throw new EstadoInvalidoException("Não é possível criar um jogo em um estado inválido.");

            return _jogo;
        }
    }
}
