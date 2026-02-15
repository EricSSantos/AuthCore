using AuthCore.Domain.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Context
{
    /// <summary>Representa fábrica de DbContext para migrations em tempo de design.</summary>
    public sealed class EFCoreDbContextFactory : IDesignTimeDbContextFactory<EFCoreDbContext>
    {
        public EFCoreDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var databaseSettings = configuration.GetSection("Database").Get<DatabaseSettings>();
            if (databaseSettings?.Postgres is null || string.IsNullOrWhiteSpace(databaseSettings.Postgres.ConnectionString))
                throw new InvalidOperationException("A string de conexão do PostgreSQL não foi definida.");

            var options = new DbContextOptionsBuilder<EFCoreDbContext>()
                .UseNpgsql(databaseSettings.Postgres.ConnectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName);
                })
                .Options;

            return new EFCoreDbContext(options);
        }
    }
}
