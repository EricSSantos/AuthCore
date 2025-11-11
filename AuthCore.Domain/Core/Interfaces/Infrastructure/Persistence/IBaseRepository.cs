using AuthCore.Domain.Core.Interfaces.Base;
using System.Linq.Expressions;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence
{
    /// <summary>
    /// Define o contrato base para repositórios de agregados persistentes.
    /// </summary>
    public interface IBaseRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : IAggregateRoot
    { }

    /// <summary>
    /// Define as operações de leitura genéricas para um repositório de agregados.
    /// </summary>
    public interface IReadRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Obtém uma entidade pelo identificador único (<c>Id</c>).
        /// Retorna <see langword="null"/> caso não seja encontrada.
        /// </summary>
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Verifica se existe alguma entidade que atenda ao critério informado.
        /// </summary>
        Task<bool> Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Conta quantas entidades atendem ao critério informado.
        /// </summary>
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }

    /// <summary>
    /// Define as operações de escrita genéricas para um repositório de agregados.
    /// </summary>
    public interface IWriteRepository<T> where T : IAggregateRoot
    {
        /// <summary>
        /// Adiciona uma nova entidade ao repositório.
        /// </summary>
        Task Add(T entity);

        /// <summary>
        /// Atualiza uma entidade existente no repositório.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Remove uma entidade existente do repositório.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Persiste todas as alterações realizadas no contexto atual.
        /// </summary>
        Task SaveChanges();
    }
}
