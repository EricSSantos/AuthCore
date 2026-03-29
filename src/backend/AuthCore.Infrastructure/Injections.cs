using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Notifications;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Persistence.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Context;
using PostgreSqlConfirmCodeRepository = AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories.ConfirmCodeRepository;
using PostgreSqlSessionRepository = AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories.SessionRepository;
using AuthCore.Infrastructure.Services.Workers;
using AuthCore.Infrastructure.Services.Web;
using UserRepository = AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories.UserRepository;
using AuthCore.Infrastructure.Persistence.Redis.Services;
using AuthCore.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AuthCore.Infrastructure
{
    /// <summary>Representa registro de dependências da infraestrutura.</summary>
    public static class Injections
    {
        /// <summary>Operação para registrar toda a infraestrutura da aplicação.</summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
            return services
                .AddPostgreSqlConnection()
                .AddRedis()
                .AddRepositories()
                .AddInfrastructureServices();
        }

        #region PostgreSQL

        /// <summary>Operação para adicionar o contexto ADO para acesso ao PostgreSQL.</summary>
        private static IServiceCollection AddPostgreSqlConnection(this IServiceCollection services)
        {
            services.AddDbContext<EFCoreDbContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var postgres = settings.Postgres;

                if (string.IsNullOrWhiteSpace(postgres.ConnectionString))
                    throw new InvalidOperationException("A string de conexão do PostgreSQL não foi definida.");

                options.UseNpgsql(postgres.ConnectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName);
                    npgsql.CommandTimeout(postgres.CommandTimeoutSeconds);
                    npgsql.EnableRetryOnFailure(
                        postgres.RetryCount,
                        TimeSpan.FromMilliseconds(postgres.RetryDelayMilliseconds),
                        errorCodesToAdd: null);
                });
            });

            services.AddSingleton<AdoDbContext>();
            return services;
        }

        #endregion

        #region Redis

        /// <summary>Operação para configurar o Redis para cache e armazenamento.</summary>
        private static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var redis = settings.Redis;

                if (string.IsNullOrWhiteSpace(redis.ConnectionString))
                    throw new InvalidOperationException("A string de conexão do Redis não foi definida.");

                return ConnectionMultiplexer.Connect(redis.ConnectionString);
            });

            return services;
        }

        #endregion

        #region Repositories

        /// <summary>Operação para registrar os repositórios da aplicação.</summary>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, PostgreSqlSessionRepository>();
            services.AddScoped<IConfirmCodeRepository, PostgreSqlConfirmCodeRepository>();
            return services;
        }

        #endregion

        #region Infrastructure Services

        /// <summary>Operação para registrar serviços auxiliares da infraestrutura.</summary>
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ISecureKeyGenerator, SecureKeyGenerator>();
            services.AddSingleton<IEcdsaProvider, EcdsaSigner>();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<ISessionState, SessionState>();
            services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();
            services.AddSingleton<IConfirmCodeAbuseGuard, ConfirmCodeAbuseGuard>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddHostedService<SessionCleanupWorker>();
            services.AddHostedService<ConfirmCodeCleanupWorker>();
            return services;
        }

        #endregion
    }
}
