namespace TechChallenge.Games.Command.Domain.Exceptions
{
    public sealed class EstadoInvalidoException : DomainException
    {
        public EstadoInvalidoException(string message) : base(message) { }
    }
}
