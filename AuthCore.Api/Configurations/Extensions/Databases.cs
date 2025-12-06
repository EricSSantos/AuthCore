using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura os bancos de dados da aplicação.
    /// </summary>
    public static class Databases
    {
        /// <summary>
        /// Registra PostgreSQL e Redis.
        /// </summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        public static void AddDatabases(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));
            builder.AddPostgreSql();
            builder.AddRedis();
        }

        #region PostgreSQL

        /// <summary>
        /// Configura o EF Core com PostgreSQL.
        /// </summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        private static void AddPostgreSql(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var connectionString = settings.Postgres.ConnectionString;

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("A string de conexão do PostgreSQL não foi definida.");

                options.UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }

        #endregion

        #region Redis

        /// <summary>
        /// Configura o Redis para cache.
        /// </summary>
        /// <param name="builder">Instância para configurar serviços.</param>
        private static void AddRedis(this WebApplicationBuilder builder)
        {
            var databaseSettings = builder.Configuration
                .GetSection("Database")
                .Get<DatabaseSettings>() ?? new DatabaseSettings();

            var redis = databaseSettings.Redis;

            if (string.IsNullOrWhiteSpace(redis.ConnectionString))
                throw new InvalidOperationException("A string de conexão do Redis não foi definida.");

            var multiplexer = ConnectionMultiplexer.Connect(redis.ConnectionString);
            builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis.ConnectionString;
            });
        }

        #endregion
    }
}
