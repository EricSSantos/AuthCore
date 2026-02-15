using AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces;
using AuthCore.Domain.Aggregates.Notifications.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Notifications;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using AuthCore.Infrastructure.Web;
using AuthCore.Infrastructure.Notifications.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.ADO.Context;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Services;
using AuthCore.Infrastructure.Security;
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
                .AddRedis(configuration)
                .AddRepositories()
                .AddInfrastructureServices();
        }

        #region PostgreSQL

        /// <summary>Operação para adicionar o contexto ADO para acesso ao PostgreSQL.</summary>
        private static IServiceCollection AddPostgreSqlConnection(this IServiceCollection services)
        {
            services.AddSingleton<AdoDbContext>();

            return services;
        }

        #endregion

        #region Redis

        /// <summary>Operação para configurar o Redis para cache e armazenamento.</summary>
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

        /// <summary>Operação para registrar os repositórios da aplicação.</summary>
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();
            return services;
        }

        #endregion

        #region Infrastructure Services

        /// <summary>Operação para registrar serviços auxiliares da infraestrutura.</summary>
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
            services.AddSingleton<IConfirmCodeAbuseGuard, ConfirmCodeAbuseGuard>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailSender, EmailSender>();
            return services;
        }

        #endregion
    }
}
