using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;
using AuthCore.Domain.Aggregates.EmailAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Messaging;
using AuthCore.Domain.Commons.Interfaces.Persistence;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;
using AuthCore.Domain.Commons.Interfaces.Security.Signing;
using AuthCore.Infrastructure.Http;
using AuthCore.Infrastructure.Messaging.RabbitMq;
using AuthCore.Infrastructure.Persistence.PostgreSQL.Repositories;
using AuthCore.Infrastructure.Persistence.Redis.Repositories;
using AuthCore.Infrastructure.Security.Hashing;
using AuthCore.Infrastructure.Security.Jwt;
using AuthCore.Infrastructure.Security.Signing;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services
                .AddInfrastructureServices()
                .AddRepositories();

            return services;
        }

        #region Infrastructure

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Http
            services.AddScoped<ICookie, CookieService>();
            services.AddScoped<IDevice, DeviceService>();

            // Security
            services.AddScoped<IPasswordHash, BCryptService>();
            services.AddScoped<IHmac, HmacService>();
            services.AddScoped<IEcdsaSigner, EcdsaService>();
            services.AddScoped<IEcdsaProvider, EcdsaService>();
            services.AddScoped<IEntropy, EntropyService>();
            services.AddScoped<IAccessToken, JwtService>();

            // Messaging
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // PostgreSQL Repositories
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Redis Repositories
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IConfirmCodeRepository, ConfirmCodeRepository>();

            return services;
        }

        #endregion
    }
}
