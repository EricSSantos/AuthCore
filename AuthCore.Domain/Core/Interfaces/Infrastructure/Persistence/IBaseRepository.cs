using AuthCore.Domain.Core.Interfaces.Base;
using System.Linq.Expressions;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence
{
    /// <summary>
    /// Define o contrato base para repositórios de agregados.
    /// </summary>
    public interface IBaseRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : IAggregateRoot
    { }

    /// <summary>
    /// Define operações de leitura para repositórios de agregados.
    /// </summary>
    public interface IReadRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Obtém a entidade pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da entidade.</param>
        /// <returns>Entidade encontrada ou null.</returns>
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Verifica se existe entidade que atenda ao critério.
        /// </summary>
        /// <param name="predicate">Expressão de filtragem.</param>
        /// <returns>True quando existir correspondência.</returns>
        Task<bool> Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Conta quantas entidades atendem ao critério.
        /// </summary>
        /// <param name="predicate">Expressão de filtragem.</param>
        /// <returns>Total de entidades encontradas.</returns>
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }

    /// <summary>
    /// Define operações de escrita para repositórios de agregados.
    /// </summary>
    public interface IWriteRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Adiciona nova entidade ao repositório.
        /// </summary>
        /// <param name="entity">Entidade a adicionar.</param>
        Task Add(T entity);

        /// <summary>
        /// Atualiza a entidade existente.
        /// </summary>
        /// <param name="entity">Entidade a atualizar.</param>
        void Update(T entity);

        /// <summary>
        /// Remove a entidade existente.
        /// </summary>
        /// <param name="entity">Entidade a remover.</param>
        void Delete(T entity);

        /// <summary>
        /// Persiste as alterações realizadas.
        /// </summary>
        Task SaveChanges();
    }
}
