using AuthCore.Domain.Core.Interfaces.Base;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IAggregateRoot
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _set;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        #region Read

        public async Task<T?> GetById(Guid id)
        {
            var property = typeof(T).GetProperty("Id");
            if (property == null)
                throw new InvalidOperationException($"A entidade {typeof(T).Name} não possui uma propriedade pública 'Id'.");

            return await _set
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            return await _set.AnyAsync(predicate);
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await _set.CountAsync(predicate);
        }

        #endregion

        #region Write

        public async Task Add(T entity)
        {
            await _set.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
