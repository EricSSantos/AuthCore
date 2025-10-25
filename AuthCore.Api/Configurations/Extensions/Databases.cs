using AuthCore.Domain.Commons.Settings;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class Databases
    {
        /// <summary>
        /// Registra os bancos de dados e provedores de persistência no container de injeção de dependência.
        /// Inclui a configuração do PostgreSQL e do Redis com base nas seções definidas em <c>appsettings.json</c>.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configurar os serviços.</param>
        /// <exception cref="InvalidOperationException">Lançada quando as strings de conexão não estão definidas.</exception>
        public static void AddDatabases(this WebApplicationBuilder builder)
        {
            // Registra a seção "Database" como fortemente tipada
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));

            // Configura provedores
            builder.AddPostgreSql();
            builder.AddRedis();
        }

        #region PostgreSQL

        /// <summary>
        /// Registra e configura o provedor do Entity Framework Core para PostgreSQL,
        /// utilizando o contexto <see cref="AppDbContext"/> e a connection string definida nas configurações.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configuração do serviço.</param>
        /// <exception cref="InvalidOperationException">Lançada quando a string de conexão do PostgreSQL não está definida.</exception>
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
        /// Registra e configura a conexão com o Redis, permitindo o uso de cache distribuído e repositórios em memória.
        /// </summary>
        /// <param name="builder">Instância do <see cref="WebApplicationBuilder"/> utilizada para configuração do serviço.</param>
        /// <exception cref="InvalidOperationException">Lançada quando a string de conexão do Redis não está definida.</exception>
        private static void AddRedis(this WebApplicationBuilder builder)
        {
            var databaseSettings = builder.Configuration
                .GetSection("Database")
                .Get<DatabaseSettings>() ?? new DatabaseSettings();

            var redis = databaseSettings.Redis;

            if (string.IsNullOrWhiteSpace(redis.ConnectionString))
                throw new InvalidOperationException("A string de conexão do Redis não foi definida.");

            // Cria o multiplexer
            var multiplexer = ConnectionMultiplexer.Connect(redis.ConnectionString);
            builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            // Configura o cache Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redis.InstanceName;
                options.Configuration = redis.ConnectionString;
            });
        }

        #endregion
    }
}
