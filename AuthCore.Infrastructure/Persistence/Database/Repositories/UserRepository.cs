using AuthCore.Domain.Commons.Interfaces.Repositories;
using AuthCore.Domain.Entities;
using AuthCore.Infrastructure.Persistence.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthCore.Infrastructure.Persistence.Database.Repositories
{
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) { }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
        }
    }
}
