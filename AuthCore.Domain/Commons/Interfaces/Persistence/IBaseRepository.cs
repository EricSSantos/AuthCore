using AuthCore.Domain.Shared;
using System.Linq.Expressions;

namespace AuthCore.Domain.Commons.Interfaces.Persistence
{
    public interface IBaseRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : Entity
    { }

    public interface IReadRepository<T> where T : Entity
    {
        /// <summary>
        /// Obtém uma entidade pelo seu identificador único.
        /// </summary>
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Verifica se existe alguma entidade que atenda ao critério especificado.
        /// </summary>
        Task<bool> Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Conta quantas entidades atendem ao critério especificado.
        /// </summary>
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }

    public interface IWriteRepository<T> where T : Entity
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
        /// Remove uma entidade do repositório.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Persiste as alterações no repositório.
        /// </summary>
        Task SaveChanges();
    }
}
