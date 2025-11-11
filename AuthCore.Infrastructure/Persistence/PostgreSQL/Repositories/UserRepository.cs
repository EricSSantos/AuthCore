using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public sealed class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) { }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant());
        }
    }
}
