using AuthCore.Domain.Shared;
using System.Linq.Expressions;

namespace AuthCore.Domain.Commons.Interfaces.Repositories
{
    public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : Entity
    { }

    public interface IReadRepository<T> where T : Entity
    {
        Task<T?> GetById(Guid id);
        Task<bool> Exists(Expression<Func<T, bool>> predicate);
        Task<int> Count(Expression<Func<T, bool>> predicate);
    }

    public interface IWriteRepository<T> where T : Entity
    {
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChanges();
    }
}
