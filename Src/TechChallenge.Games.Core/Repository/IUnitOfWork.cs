namespace TechChallenge.Games.Core.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IJogoRepository JogoRepository { get; }
        Task CommitAsync();
    }
}
