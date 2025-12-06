using AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces;
using AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Http;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Messaging;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Infrastructure
{
    public static class Injections
    {
        /// <summary>
        /// Registra toda a infraestrutura da aplicação.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

            return services
                .AddPostgreSqlConnection()
                .AddRedis(configuration)
                .AddRepositories()
                .AddInfrastructureServices();
        }

        #region PostgreSQL

        /// <summary>
        /// Adiciona o contexto ADO para acesso ao PostgreSQL.
        /// </summary>
        private static IServiceCollection AddPostgreSqlConnection(this IServiceCollection services)
        {
            services.AddSingleton<AdoDbContext>(sp =>
            {
                IOptions<DatabaseSettings> options = sp.GetRequiredService<IOptions<DatabaseSettings>>();
                DatabaseSettings databaseSettings = options.Value;

                if (databaseSettings.Postgres is null ||
                    string.IsNullOrWhiteSpace(databaseSettings.Postgres.ConnectionString))
                    throw new InvalidOperationException("A string de conexão do PostgreSQL não foi definida.");

                return new AdoDbContext(databaseSettings.Postgres.ConnectionString);
            });

            return services;
        }

        #endregion

        #region Redis

        /// <summary>
        /// Configura o Redis para cache e armazenamento.
        /// </summary>
        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            DatabaseSettings databaseSettings = configuration
                .GetSection("Database")
                .Get<DatabaseSettings>() ?? new DatabaseSettings();

            var redis = databaseSettings.Redis;

            if (string.IsNullOrWhiteSpace(redis.ConnectionString))
                throw new InvalidOperationException("A string de conexão do Redis não foi definida.");

            IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redis.ConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis.ConnectionString;
            });

            return services;
        }

        #endregion

        #region Repositories

        /// <summary>
        /// Registra os repositórios da aplicação.
        /// </summary>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();
            return services;
        }

        #endregion

        #region Infrastructure Services

        /// <summary>
        /// Registra serviços auxiliares da infraestrutura.
        /// </summary>
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISecureKeyGenerator, SecureKeyGenerator>();
            services.AddScoped<IEcdsaSigner, EcdsaSigner>();
            services.AddScoped<IEcdsaProvider, EcdsaSigner>();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<ISessionState, SessionState>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailPublisher, EmailPublisher>();
            return services;
        }

        #endregion
    }
}
