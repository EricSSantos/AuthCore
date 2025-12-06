using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Context
{
    /// <summary>
    /// Representa o contexto de dados da aplicação.
    /// </summary>
    public sealed partial class EFCoreDbContext : DbContext
    {
        #region DbSets

        /// <summary>
        /// Mapeia a entidade User no banco de dados.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor usado em cenários onde as opções são fornecidas externamente (tests, etc.).
        /// </summary>
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Construtor sem parâmetros usado pelo EF Core em tempo de design (migrations).
        /// </summary>
        public EFCoreDbContext()
        {
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Configura o DbContext quando nenhuma configuração foi fornecida externamente.
        /// Usado principalmente em tempo de design para migrations.
        /// </summary>
        /// <param name="optionsBuilder">Builder de opções do contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            DatabaseSettings? databaseSettings = configuration
                .GetSection("Database")
                .Get<DatabaseSettings>();

            if (databaseSettings is null || databaseSettings.Postgres is null)
                throw new InvalidOperationException("As configurações de Database:Postgres não foram definidas.");

            string connectionString = databaseSettings.Postgres.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("A string de conexão do PostgreSQL não foi definida.");

            optionsBuilder.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName);
            });
        }

        #endregion

        #region Model Configuration

        /// <summary>
        /// Configura o modelo aplicando mappings e extensões parciais.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFCoreDbContext).Assembly);
            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Permite acrescentar configurações adicionais ao modelo.
        /// </summary>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion
    }
}
