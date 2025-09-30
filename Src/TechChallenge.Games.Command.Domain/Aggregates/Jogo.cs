using TechChallenge.Games.Command.Domain.Builders;
using TechChallenge.Games.Command.Domain.Events;
using TechChallenge.Games.Command.Domain.Exceptions;

namespace TechChallenge.Games.Command.Domain.Aggregates
{
    public sealed class Jogo : BaseAggregate
    {
        #region Propriedades

        public string Nome { get; internal set; }
        public string Descricao { get; internal set; }
        public DateTime DataLancamento { get; internal set; }
        public decimal Preco { get; internal set; }
        public int Desconto { get; internal set; }
        public decimal Valor => Preco - (Preco * Desconto / 100);

        #endregion

        #region Construtor

        internal Jogo() { }

        public static JogoBuilder Novo => new JogoBuilder();

        #endregion

        #region Métodos Públicos

        public void AlterarDados(string? nome, string? descricao, DateTime? dataLancamento)
        {
            if (string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(descricao) && dataLancamento == null)
                throw new ArgumentException("Pelo menos um dos parâmetros deve ser fornecido para alterar os dados do jogo.");
            if (!string.IsNullOrWhiteSpace(nome))
                Nome = nome;
            if (!string.IsNullOrWhiteSpace(descricao))
                Descricao = descricao;
            if (dataLancamento != null)
                DataLancamento = dataLancamento.Value;

            RaiseEvent(new DadosAlteradosEvent
            {
                AggregateId = Id,
                Nome = nome,
                Descricao = descricao,
                DataLancamento = dataLancamento
            });
        }

        public void AlterarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.", nameof(novoPreco));

            RaiseEvent(new PrecoAlteradoEvent
            {
                AggregateId = Id,
                NovoPreco = novoPreco
            });
        }

        public void AplicarDesconto(int percentual)
        {
            if (percentual < 0 || percentual > 100)
                throw new ArgumentException("O percentual de desconto deve estar entre 0 e 100.", nameof(percentual));

            Desconto = percentual;

            RaiseEvent(new DescontoAplicadoEvent
            {
                AggregateId = Id,
                Percentual = percentual
            });
        }

        public void Remover()
        {
            if (Deletado)
                throw new InvalidOperationException("O jogo já foi removido.");

            RaiseEvent(new JogoRemovidoEvent
            {
                AggregateId = Id
            });
        }

        #endregion

        #region Eventos

        protected override void Apply(BaseEvent @event)
        {
            switch (@event)
            {
                case JogoCriadoEvent e:
                    Apply(e);
                    break;
                case DadosAlteradosEvent e:
                    Apply(e);
                    break;
                case PrecoAlteradoEvent e:
                    Apply(e);
                    break;
                case DescontoAplicadoEvent e:
                    Apply(e);
                    break;
                case JogoRemovidoEvent e:
                    Apply(e);
                    break;
                default:
                    throw new EventoInvalidoException<Jogo>(@event);
            }
        }

        private void Apply(JogoCriadoEvent @event)
        {
            Id = @event.AggregateId;
            Nome = @event.Nome;
            DataLancamento = @event.DataLancamento;
            Preco = @event.Preco;
        }

        private void Apply(DadosAlteradosEvent @event)
        {
            if (!string.IsNullOrWhiteSpace(@event.Nome))
                Nome = @event.Nome;
            if (!string.IsNullOrWhiteSpace(@event.Descricao))
                Descricao = @event.Descricao;
            if (@event.DataLancamento != null)
                DataLancamento = @event.DataLancamento.Value;
        }

        private void Apply(PrecoAlteradoEvent @event)
        {
            Preco = @event.NovoPreco;
        }

        private void Apply(DescontoAplicadoEvent @event)
        {
            Desconto = @event.Percentual;
        }

        private void Apply(JogoRemovidoEvent @event)
        {
            Deletado = true;
        }

        #endregion
    }
}
