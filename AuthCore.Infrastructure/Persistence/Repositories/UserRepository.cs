using AuthCore.Domain.Entities;
using AuthCore.Domain.Interfaces.Repositories;
using AuthCore.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthCore.Infrastructure.Persistence.Repositories
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
