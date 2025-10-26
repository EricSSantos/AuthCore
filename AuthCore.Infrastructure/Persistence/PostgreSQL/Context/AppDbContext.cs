using AuthCore.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.Context
{
    public sealed partial class AppDbContext : DbContext
    {
        #region DbSets

        public DbSet<User> Users { get; set; } = null!;

        #endregion

        #region Constructors

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        #endregion

        #region Model Configuration

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion
    }
}
