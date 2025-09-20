using TechChallenge.Games.Core.Entity;

namespace TechChallenge.Games.Core.Repository;

public interface IRepository<T> where T : EntityBase
{
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<T?> ObterPorIdAsync(int id);
    void Cadastrar(T entidade);
    void Alterar(T entidade);
    void Deletar(T entidade);
}